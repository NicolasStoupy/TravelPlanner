/*
--------------------------------------------------------------------------------
 Script Name  : V2024055032236_Add_ActivityDate_To_Activity.sql
 Date         : 2025-05-03
 Author       : Nicolas Stoupy
 Description  : 
     Ce script ajoute une colonne [ActivityDate] de type [date] � la table [dbo].[Activity].
     Cette colonne est con�ue pour enregistrer la date planifi�e de chaque activit�.

 �tapes principales :
    Ajout de la colonne [ActivityDate] en tant que colonne nullable (facultative).
--------------------------------------------------------------------------------
*/

ALTER TABLE [dbo].[Activity]
ADD [ActivityDate] [date] NULL;