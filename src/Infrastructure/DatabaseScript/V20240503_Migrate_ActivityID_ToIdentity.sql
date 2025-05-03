/*
--------------------------------------------------------------------------------
 Script Name  : Migrate_ActivityID_ToIdentity.sql
 Date         : 2025-05-03
 Author       : Nicolas Stoupy
 Description  : 
     Ce script convertit la colonne [ActivityID] de la table [dbo].[Activity] 
     en une colonne auto-incrémentée (IDENTITY(1,1)) tout en conservant les données 
     et l'intégrité référentielle avec les tables dépendantes.

 Étapes principales :
     1. Suppression temporaire des clés étrangères pointant vers [Activity].
     2. Suppression de la clé primaire existante.
     3. Ajout d'une nouvelle colonne [ActivityID_New] avec IDENTITY.
     4. Création d'une table de correspondance (#ActivityMapping) pour mapper les anciens et nouveaux ID.
     5. Mise à jour des tables dépendantes : [Attendee], [ActivityCost], [LogBook].
     6. Suppression de la colonne [ActivityID] existante.
     7. Renommage de [ActivityID_New] en [ActivityID].
     8. Recréation de la clé primaire et des contraintes étrangères.
     9. Suppression de la table temporaire.

 Attention :
     - Sauvegarder la base avant exécution.
     - À exécuter dans un environnement de test avant production.
     - Ce script suppose que [TripID, ActivityID] est une combinaison unique.

--------------------------------------------------------------------------------
*/
-- Étape 1 : Supprimer les clés étrangères dépendantes
ALTER TABLE [dbo].[Attendee] DROP CONSTRAINT [FK_Attendee_Activity];
ALTER TABLE [dbo].[ActivityCost] DROP CONSTRAINT [FK_ActivityCost_Activity];
ALTER TABLE [dbo].[LogBook] DROP CONSTRAINT [FK_LogBook_Activity];

-- Étape 2 : Supprimer la clé primaire
ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [PK_Activity];

-- Étape 3 : Ajouter une nouvelle colonne auto-incrémentée
ALTER TABLE [dbo].[Activity] ADD ActivityID_New INT IDENTITY(1,1) NOT NULL;

-- Étape 4 : Mettre à jour les tables dépendantes avec les nouvelles valeurs
-- On commence par créer une table de correspondance temporaire

SELECT TripID, ActivityID AS OldID, ActivityID_New INTO #ActivityMapping
FROM [dbo].[Activity];

-- Mettre à jour les tables dépendantes
UPDATE att
SET att.ActivityID = map.ActivityID_New
FROM [dbo].[Attendee] att
JOIN #ActivityMapping map ON att.TripID = map.TripID AND att.ActivityID = map.OldID;

UPDATE cost
SET cost.ActivityID = map.ActivityID_New
FROM [dbo].[ActivityCost] cost
JOIN #ActivityMapping map ON cost.TripID = map.TripID AND cost.ActivityID = map.OldID;

UPDATE log
SET log.ActivityID = map.ActivityID_New
FROM [dbo].[LogBook] log
JOIN #ActivityMapping map ON log.TripID = map.TripID AND log.ActivityID = map.OldID;

-- Étape 5 : Supprimer l’ancienne colonne
ALTER TABLE [dbo].[Activity] DROP COLUMN ActivityID;

-- Étape 6 : Renommer la colonne IDENTITY
EXEC sp_rename 'dbo.Activity.ActivityID_New', 'ActivityID', 'COLUMN';

-- Étape 7 : Recréer la clé primaire
ALTER TABLE [dbo].[Activity]
ADD CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED ([TripID], [ActivityID]);

-- Étape 8 : Recréer les clés étrangères
ALTER TABLE [dbo].[Attendee]
ADD CONSTRAINT [FK_Attendee_Activity]
FOREIGN KEY ([TripID], [ActivityID])
REFERENCES [dbo].[Activity] ([TripID], [ActivityID]);

ALTER TABLE [dbo].[ActivityCost]
ADD CONSTRAINT [FK_ActivityCost_Activity]
FOREIGN KEY ([TripID], [ActivityID])
REFERENCES [dbo].[Activity] ([TripID], [ActivityID]);

ALTER TABLE [dbo].[LogBook]
ADD CONSTRAINT [FK_LogBook_Activity]
FOREIGN KEY ([TripID], [ActivityID])
REFERENCES [dbo].[Activity] ([TripID], [ActivityID]);

-- Nettoyage
DROP TABLE #ActivityMapping;
