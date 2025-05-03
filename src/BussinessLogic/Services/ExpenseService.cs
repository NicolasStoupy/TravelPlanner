using BussinessLogic.Interfaces;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;


namespace BussinessLogic.Services
{
    public class ExpenseService : IExpenseService
    {


        private readonly IDbContextFactory<TravelPlannerContext> _context;

        public ExpenseService(IDbContextFactory<TravelPlannerContext> travelPlannerContext)
        {
            _context = travelPlannerContext;
        }

        public List<string> GetCurrencies()
        {
            using var context = _context.CreateDbContext();
            return context.Currencies.Select(c => c.CurrencyCode).ToList();
        }


    }
}
