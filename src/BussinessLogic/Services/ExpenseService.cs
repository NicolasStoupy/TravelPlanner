using BussinessLogic.Interfaces;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;

namespace BussinessLogic.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context;

        public ExpenseService(IDbContextFactory<TravelPlannerContext> travelPlannerContext)
        {
            _context = travelPlannerContext;
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
    }
}