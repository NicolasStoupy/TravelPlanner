using AutoMapper;
using BussinessLogic.Entities;
using Commons.Models;
using Commons.Resources;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BussinessLogic.Processors
{
    public class TripTbinProcessor : TbinProcessor<Trip>
    {
 
        private readonly IDbContextFactory<TravelPlannerContext> _contextFactory;
  

        public TripTbinProcessor(
            IDbContextFactory<TravelPlannerContext> contextFactory
        )
        {
            _contextFactory = contextFactory;
          
        }

        /// <summary>
        /// Désérialise le JSON en entité Trip (avec ses collections), remet tous les IDs à zéro,
        /// puis ajoute ces entités en base pour créer de nouveaux enregistrements.
        /// </summary>
        /// <param name="tbinFile">Tableau d’octets contenant le JSON UTF-8 du voyage à importer.</param>
        /// <returns>Le Trip nouvellement enregistré (avec ses IDs auto-générés), ou null si échec.</returns>
        public override Trip ConvertTbinToTrip(byte[] tbinFile)
        {
            if (tbinFile == null || tbinFile.Length == 0)
            {
               
                return null;
            }

            try
            {
                // 1) Désérialiser le JSON en entité Trip incluant ses relations
                string json = Encoding.UTF8.GetString(tbinFile);
                var importedTrip = JsonSerializer.Deserialize<Trip>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (importedTrip == null)
                {
                   
                    return null;
                }

                // 2) Remettre tous les IDs à zéro pour forcer EF à générer de nouveaux enregistrements
                ResetTripAndChildrenIds(importedTrip);

                // 3) Ajouter en base
                using var ctx = _contextFactory.CreateDbContext();
                // On veut persister Trip + liée Activities + ActivityCosts + Attendees + LogBooks + Media
                // Comme l’entité Trip possède des collections navigables, EF va cascade‐inserer.
                ctx.Trips.Add(importedTrip);
                ctx.SaveChanges();

           

                return importedTrip;
            }
            catch (JsonException jex)
            {
                
                return null;
            }
            catch (DbUpdateException dbEx)
            {
              
                return null;
            }
            catch (Exception ex)
            {
              
                return null;
            }
        }

     

        public override byte[] ConvertToTbin(Trip trip)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parcourt l’entité Trip et toutes ses relations (Activities, ActivityCosts, Attendees, LogBooks, Media)
        /// pour remettre chaque clé primaire et clé étrangère à 0 ou à null, garantissant l’insertion de nouveaux enregistrements.
        /// </summary>
        private void ResetTripAndChildrenIds(Trip trip)
        {
            // 1) Reset TripId (PK)
            trip.TripId = 0;

            // 2) Si Trip possède une collection Media (table Medium)
            if (trip.Media != null)
            {
                foreach (var medium in trip.Media)
                {
                    medium.MediaId = 0;
                    medium.TripId = 0;
                    // medium.ActivityCostId reste tel quel si null, sinon reset:
                    if (medium.ActivityCostId != null)
                        medium.ActivityCostId = 0;
                    // Note : FileGuid, Description, UploadedAt, etc. restent inchangés
                }
            }

            // 3) Si Trip possède des LogBooks (LogBook)
            if (trip.LogBooks != null)
            {
                foreach (var log in trip.LogBooks)
                {
                    log.LogBookId = 0;
                    log.TripLogBook = 0; // FK vers Trip.TripId, remis à zéro
                    // Si lié à une activité : 
                    if (log.ActivityId != null)
                        log.ActivityId = 0;
                }
            }

            // 4) Si Trip possède des Activities
            if (trip.Activities != null)
            {
                foreach (var activity in trip.Activities)
                {
                    // Composite PK : TripId + ActivityId
                    activity.ActivityId = 0;
                    activity.TripId = 0; // FK vers Trip
                    // Reset Sequence ? (généralement recalculé)
                    // activity.Sequence = 0; // ou conserver la valeur si l’ordre est nécessaire

                    // 4.1) ActivityCosts pour cette activité
                    if (activity.ActivityCosts != null)
                    {
                        foreach (var cost in activity.ActivityCosts)
                        {
                            cost.ActivityCostId = 0;
                            cost.ActivityId = 0;
                            cost.TripId = 0; // FK vers Trip
                                             // cost.CurrencyCode reste le même (utilisé comme FK vers Currency)
                                             // cost.CreatedAt et UpdatedAt seront écrasés par la BDD si défaut défini

                            // 4.1.1) Media attachés à un ActivityCost
                            if (cost.Media != null)
                            {
                                foreach (var media in cost.Media)
                                {
                                    media.MediaId = 0;
                                    media.ActivityCostId = 0;
                                    media.TripId = 0;
                                    // media.FileGuid reste inchangé
                                }
                            }
                        }
                    }

                    // 4.2) Attendees (participants) de cette activité
                    if (activity.Attendees != null)
                    {
                        foreach (var attendee in activity.Attendees)
                        {
                            attendee.AttendeeId = 0;
                            attendee.ActivityId = 0;
                            attendee.TripId = 0;
                            // attendee.Email, Name, etc. restent inchangés
                        }
                    }

                    // 4.3) LogBooks attachés à une activité (optionnel si existants)
                    if (activity.LogBooks != null)
                    {
                        foreach (var actLog in activity.LogBooks)
                        {
                            actLog.LogBookId = 0;
                            actLog.ActivityId = 0;
                            actLog.TripLogBook = 0; // Pas lié au Trip, mais à TripLogBook FK
                        }
                    }
                }
            }
        }
    }
}
