using BussinessLogic.Interfaces;

namespace BussinessLogic.Services
{
    public class ApplicationService : IApplicationService
    {
        public IExpenseService ExpenseService { get; }
        public ITripService TripService { get; }

        public ApplicationService(IExpenseService expenseService, ITripService tripService)
        {
            ExpenseService = expenseService;
            TripService = tripService;
        }
    }
}
