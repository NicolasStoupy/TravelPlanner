using BussinessLogic.Interfaces;

namespace BussinessLogic.Services
{
    public class ApplicationService : IApplicationService
    {
        public IExpenseService ExpenseService { get; }
        public ITravelService TravelService { get; }

        public IMediaService MediaService { get; }

        public IActivityService ActivityService { get; }

        public ILogBookService LogBookService { get; }

        public ApplicationService(IExpenseService expenseService, ITravelService tripService, IMediaService mediaService, IActivityService activityService, ILogBookService logBookService)
        {
            ExpenseService = expenseService;
            TravelService = tripService;
            MediaService = mediaService;
            ActivityService = activityService;
            LogBookService = logBookService;
           
        }
    }
}
