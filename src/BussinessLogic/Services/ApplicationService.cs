using BussinessLogic.Interfaces;

namespace BussinessLogic.Services
{
    public class ApplicationService : IApplicationService
    {
        public IExpenseService ExpenseService { get; }
        public ITravelService TravelService { get; }

        public IMediaService MediaService { get; }

        public IActivityService ActivityService { get; }

        public ApplicationService(IExpenseService expenseService, ITravelService tripService, IMediaService mediaService, IActivityService activityService)
        {
            ExpenseService = expenseService;
            TravelService = tripService;
            MediaService = mediaService;
            ActivityService = activityService;
        }
    }
}
