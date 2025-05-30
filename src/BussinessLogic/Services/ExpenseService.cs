using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Extensions;
using BussinessLogic.Interfaces;
using Commons.Models;
using Commons.Resources;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BussinessLogic.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context;
        private readonly IMapper _mapper;
        private readonly DocumentProvider _documentProvider;
        private readonly ILogger<ExpenseService> _logger;



        public ExpenseService(IDbContextFactory<TravelPlannerContext> travelPlannerContext, IMapper mapper, DocumentProvider documentProvider, ILogger<ExpenseService> logger)
        {
            _context = travelPlannerContext;
            _mapper = mapper;
            _documentProvider = documentProvider;
            _logger = logger;
        }
        public async Task<ServiceResult<bool>> CreateCostAsync(int activityID, Cost newCost)
        {
            if (activityID <= 0 || newCost == null)
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.INVALID_INPUT);

            await using var ctx = _context.CreateDbContext();
            var activity = await ctx.Activities.FirstOrDefaultAsync(a=>a.ActivityId==activityID);
            if (activity == null)
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.ACTIVITY_NOT_FOUND);

            try
            {
                var entity = _mapper.Map<ActivityCost>(newCost);
                activity.ActivityCosts.Add(entity);
                await ctx.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error creating cost for ActivityID={ActivityID}", activityID);
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.DATABASE_ERROR);
            }
        }


        //public Result CreateCost(int activityID, Cost newCost)
        //{
        //    using var context = _context.CreateDbContext();
        //    var activity = context.Activities.SingleOrDefault(t => t.ActivityId == activityID);
        //    if (activity != null)
        //    {
        //        var ActivityCost = _mapper.Map<ActivityCost>(newCost);

        //        activity.ActivityCosts.Add(ActivityCost);
        //        context.SaveChanges();

        //    }
        //    return Result.Success("ok");

        //}
        public async Task<ServiceResult<List<Cost>>> GetCostAsync(int activityID)
        {
            if (activityID <= 0)
                return ServiceResult<List<Cost>>.Failure(ExpenseServiceMessages.INVALID_INPUT);

            await using var ctx = _context.CreateDbContext();

            var costs = await ctx.ActivityCosts
                .Where(ac => ac.ActivityId == activityID)
                .ToListAsync();

            if (!_mapper.TryMap(costs, out List<Cost> dto, _logger))
            {
                // Le mapping a échoué, on renvoie un ServiceResult d’échec
                return ServiceResult<List<Cost>>.Failure(ExpenseServiceMessages.UNKNOWN_ERROR);
            }
            return ServiceResult<List<Cost>>.Success(dto);

        }
        public  ServiceResult<List<string>> GetCurrencies()
        {
             using var ctx = _context.CreateDbContext();

            var list =  ctx.Currencies
                .Select(c => c.CurrencyCode)
                .ToList();
            return ServiceResult<List<string>>.Success(list);

        }

        public async Task<ServiceResult<bool>> RemoveCostAsync(int costID)
        {
            if (costID <= 0)
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.INVALID_INPUT);

            await using var ctx = _context.CreateDbContext();
            var cost = await ctx.ActivityCosts
                .Include(ac => ac.Media)
                .SingleOrDefaultAsync(ac => ac.ActivityCostId == costID);

            if (cost == null)
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.COST_NOT_FOUND);

            try
            {
                ctx.Media.RemoveRange(cost.Media);
                ctx.ActivityCosts.Remove(cost);
                await ctx.SaveChangesAsync();
                return ServiceResult<bool>.Success(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error removing cost {CostID}", costID);
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.DATABASE_ERROR);
            }
        }

        public async Task<ServiceResult<bool>> RemoveTicketAsync(Guid ticketId)
        {
            if (ticketId == Guid.Empty)
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.INVALID_INPUT);

            await using var ctx = _context.CreateDbContext();
            var media = await ctx.Media.SingleOrDefaultAsync(m => m.FileGuid == ticketId);
            if (media == null)
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.MEDIA_NOT_FOUND);


            ctx.Media.Remove(media);
            await ctx.SaveChangesAsync();
            _documentProvider.RemoveFile(ticketId, Commons.TypeMedia.Images);
            return ServiceResult<bool>.Success(true);

        }

        public async Task<ServiceResult<bool>> SaveNewCostAsync(int travelID, int costID, byte[] file)
        {
            if (travelID <= 0 || costID <= 0 || file == null || file.Length == 0)
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.INVALID_INPUT);

            await using var ctx = _context.CreateDbContext();

            var guid = _documentProvider.SaveFile(file);
            if (guid == null)
                return ServiceResult<bool>.Failure(ExpenseServiceMessages.FILE_SAVE_ERROR);

            var media = new Medium
            {
                ActivityCostId = costID,
                TripId = travelID,
                FileGuid = guid.Value,
                Description = string.Empty,
                MediaType = 1
            };
            ctx.Media.Add(media);
            await ctx.SaveChangesAsync();
            return ServiceResult<bool>.Success(true);

        }

        //    public List<Cost> GetCost(int ActivityID)
        //    {
        //        using var context = _context.CreateDbContext();
        //        var activityCosts = context.ActivityCosts.Where(ac => ac.ActivityId == ActivityID).ToList();
        //        return _mapper.Map<List<Cost>>(activityCosts);
        //    }

        //    /// <summary>
        //    /// Retrieves all available currency codes from the database.
        //    /// </summary>
        //    /// <returns>A list of currency code strings (e.g., "USD", "EUR").</returns>
        //    public List<string> GetCurrencies()
        //    {
        //        using var context = _context.CreateDbContext();
        //        return context.Currencies.Select(c => c.CurrencyCode).ToList();
        //    }

        //    public Result RemoveCost(int costID)
        //    {
        //        using var context = _context.CreateDbContext();
        //        var ActivitiCost = context.ActivityCosts.Include(i => i.Media).SingleOrDefault(c => c.ActivityCostId == costID);
        //        if (ActivitiCost != null)
        //        {
        //            var medias = ActivitiCost.Media;
        //            context.Media.RemoveRange(medias);
        //            context.Remove(ActivitiCost);
        //            context.SaveChanges();
        //        }
        //        return Result.Success("ok");
        //    }

        //    public Result RemoveTicket(Guid ticketId)
        //    {
        //        using var context = _context.CreateDbContext();

        //        var media = context.Media.SingleOrDefault(m => m.FileGuid == ticketId);
        //        if (media != null)
        //            context.Media.Remove(media);
        //        context.SaveChanges();
        //        _documentProvider.RemoveFile(ticketId, Commons.TypeMedia.Images);

        //        return Result.Success("Supprimmer");
        //    }

        //    public Result SaveNewCost(int TravelId, int costID, byte[] file)
        //    {
        //        using var context = _context.CreateDbContext();

        //        var Fileguid = _documentProvider.SaveFile(file);
        //        if (Fileguid != null)
        //        {
        //            var MediaDb = new Medium()
        //            {
        //                ActivityCostId = costID,
        //                Description = string.Empty,
        //                MediaType = 1,
        //                TripId = TravelId,
        //                FileGuid = Fileguid.Value,
        //            };

        //            context.Add(MediaDb);
        //            context.SaveChanges();
        //        }
        //        return Result.Success("Ticket Enregistré");

        //    }
        //}
    }
}