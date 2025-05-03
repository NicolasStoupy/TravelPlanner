/*
--------------------------------------------------------------------------------
 Script Name  : Migrate_ActivityID_ToIdentity.sql
 Date         : 2025-05-03
 Author       : Nicolas Stoupy
 Description  : 
     Ce script convertit la colonne [ActivityID] de la table [dbo].[Activity] 
     en une colonne auto-incr�ment�e (IDENTITY(1,1)) tout en conservant les donn�es 
     et l'int�grit� r�f�rentielle avec les tables d�pendantes.

 �tapes principales :
     1. Suppression temporaire des cl�s �trang�res pointant vers [Activity].
     2. Suppression de la cl� primaire existante.
     3. Ajout d'une nouvelle colonne [ActivityID_New] avec IDENTITY.
     4. Cr�ation d'une table de correspondance (#ActivityMapping) pour mapper les anciens et nouveaux ID.
     5. Mise � jour des tables d�pendantes : [Attendee], [ActivityCost], [LogBook].
     6. Suppression de la colonne [ActivityID] existante.
     7. Renommage de [ActivityID_New] en [ActivityID].
     8. Recr�ation de la cl� primaire et des contraintes �trang�res.
     9. Suppression de la table temporaire.

 Attention :
     - Sauvegarder la base avant ex�cution.
     - � ex�cuter dans un environnement de test avant production.
     - Ce script suppose que [TripID, ActivityID] est une combinaison unique.

--------------------------------------------------------------------------------
*/
-- �tape 1 : Supprimer les cl�s �trang�res d�pendantes
ALTER TABLE [dbo].[Attendee] DROP CONSTRAINT [FK_Attendee_Activity];
ALTER TABLE [dbo].[ActivityCost] DROP CONSTRAINT [FK_ActivityCost_Activity];
ALTER TABLE [dbo].[LogBook] DROP CONSTRAINT [FK_LogBook_Activity];

-- �tape 2 : Supprimer la cl� primaire
ALTER TABLE [dbo].[Activity] DROP CONSTRAINT [PK_Activity];

-- �tape 3 : Ajouter une nouvelle colonne auto-incr�ment�e
ALTER TABLE [dbo].[Activity] ADD ActivityID_New INT IDENTITY(1,1) NOT NULL;

-- �tape 4 : Mettre � jour les tables d�pendantes avec les nouvelles valeurs
-- On commence par cr�er une table de correspondance temporaire

SELECT TripID, ActivityID AS OldID, ActivityID_New INTO #ActivityMapping
FROM [dbo].[Activity];

-- Mettre � jour les tables d�pendantes
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

-- �tape 5 : Supprimer l�ancienne colonne
ALTER TABLE [dbo].[Activity] DROP COLUMN ActivityID;

-- �tape 6 : Renommer la colonne IDENTITY
EXEC sp_rename 'dbo.Activity.ActivityID_New', 'ActivityID', 'COLUMN';

-- �tape 7 : Recr�er la cl� primaire
ALTER TABLE [dbo].[Activity]
ADD CONSTRAINT [PK_Activity] PRIMARY KEY CLUSTERED ([TripID], [ActivityID]);

-- �tape 8 : Recr�er les cl�s �trang�res
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
