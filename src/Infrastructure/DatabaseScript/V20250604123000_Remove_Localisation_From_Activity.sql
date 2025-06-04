/*
--------------------------------------------------------------------------------
 Script Name  : V20250604123000_Remove_Localisation_From_Activity.sql
 Date         : 2025-06-04
 Author       : Stoupy Nicolas
 Description  :
     Ce script supprime la colonne [Localisation] de type [geography] 
     dans la table [dbo].[Activity]. Cette colonne n’est plus nécessaire 
     dans le modèle de données.

 Étapes principales :
    1. Vérifier que la colonne [Localisation] n’est pas utilisée dans des vues
       ou des contraintes dépendantes.
    2. Supprimer la colonne [Localisation] de la table [Activity].
--------------------------------------------------------------------------------
*/

ALTER TABLE [dbo].[Activity]
DROP COLUMN [Localisation];
