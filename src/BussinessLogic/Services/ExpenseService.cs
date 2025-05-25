using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Models;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogic.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context;
        private readonly IMapper _mapper;
        private readonly DocumentProvider _documentProvider;



        public ExpenseService(IDbContextFactory<TravelPlannerContext> travelPlannerContext, IMapper mapper, DocumentProvider documentProvider)
        {
            _context = travelPlannerContext;
            _mapper = mapper;
            _documentProvider = documentProvider;
        }

        public Result CreateCost( int activityID, Cost newCost)
        {
            using var context = _context.CreateDbContext();
            var activity = context.Activities.SingleOrDefault(t => t.ActivityId == activityID);
            if(activity != null)
            {
                var ActivityCost = _mapper.Map<ActivityCost>(newCost);

                activity.ActivityCosts.Add(ActivityCost);
                context.SaveChanges();
            
            }
            return Result.Success("ok");
          
        }

        public List<Cost> GetCost(int ActivityID)
        {
            using var context = _context.CreateDbContext();
            var activityCosts = context.ActivityCosts.Where(ac => ac.ActivityId == ActivityID).ToList();
            return _mapper.Map<List<Cost>>(activityCosts);
        }

        /// <summary>
        /// Retrieves all available currency codes from the database.
        /// </summary>
        /// <returns>A list of currency code strings (e.g., "USD", "EUR").</returns>
        public List<string> GetCurrencies()
        {
            using var context = _context.CreateDbContext();
            return context.Currencies.Select(c => c.CurrencyCode).ToList();
        }

        public Result RemoveCost(int costID)
        {
            using var context = _context.CreateDbContext();
            var ActivitiCost = context.ActivityCosts.Include(i=>i.Media).SingleOrDefault(c => c.ActivityCostId == costID);
            if(ActivitiCost != null)
            {
                var medias = ActivitiCost.Media;
                context.Media.RemoveRange(medias);
                context.Remove(ActivitiCost);
                context.SaveChanges();
            }
           return  Result.Success("ok");
        }

        public Result RemoveTicket(Guid ticketId)
        {
            using var context = _context.CreateDbContext();

            var media = context.Media.SingleOrDefault(m => m.FileGuid == ticketId);
            if(media != null)
            context.Media.Remove(media);
            context.SaveChanges();
            _documentProvider.RemoveFile(ticketId, Commons.TypeMedia.Images);

            return Result.Success("Supprimmer");
        }

        public Result SaveNewCost(int TravelId, int costID, byte[] file)
        {
            using var context = _context.CreateDbContext();

            var Fileguid = _documentProvider.SaveFile(file);
            if (Fileguid != null)
            {
                var MediaDb = new Medium()
                {
                    ActivityCostId = costID,
                    Description = string.Empty,
                    MediaType = 1,
                    TripId = TravelId,
                    FileGuid = Fileguid.Value,
                };

                context.Add(MediaDb);
                context.SaveChanges();
            }
            return Result.Success("Ticket Enregistré");

        }
    }
}