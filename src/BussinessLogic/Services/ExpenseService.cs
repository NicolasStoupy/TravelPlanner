using BussinessLogic.Interfaces;
using Infrastructure.EntityModels;


namespace BussinessLogic.Services
{
    public class ExpenseService : IExpenseService
    {


        private readonly TravelPlannerContext _context;

        public ExpenseService(TravelPlannerContext travelPlannerContext)
        {
            _context = travelPlannerContext;
        }

        public List<string> GetCurrencies()
        {
            return _context.Currencies.Select(c => c.CurrencyCode).ToList();
        }


    }
}
