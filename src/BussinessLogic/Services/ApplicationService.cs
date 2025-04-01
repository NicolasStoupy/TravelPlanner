using BussinessLogic.Interfaces;

namespace BussinessLogic.Services
{
    public class ApplicationService : IApplicationService
    {
        public IExpenseService ExpenseService { get; }
        public ITripService TripService { get; }

        public IMediaService MediaService { get; }

        public ApplicationService(IExpenseService expenseService, ITripService tripService, IMediaService mediaService)
        {
            ExpenseService = expenseService;
            TripService = tripService;
            MediaService = mediaService;
        }
    }
}
