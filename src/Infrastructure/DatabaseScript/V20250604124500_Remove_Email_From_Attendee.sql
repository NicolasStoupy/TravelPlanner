/*
--------------------------------------------------------------------------------
 Script Name  : V20250604124500_Remove_Email_From_Attendee.sql
 Date         : 2025-06-04
 Author       : Nicolas Stoupy
 Description  :
     Ce script supprime la colonne 
     dans la table [dbo].[Attendee]. Cette colonne n’est plus nécessaire
     pour la gestion des participants.

 Étapes principales :
    1. Vérifier qu’aucune contrainte ou procédure stockée ne dépend du champ [Email].
    2. Supprimer la colonne [Email] de la table [Attendee].
--------------------------------------------------------------------------------
*/

ALTER TABLE [dbo].[Attendee]
DROP COLUMN [Email];
GO
