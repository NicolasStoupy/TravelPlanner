namespace BussinessLogic.Interfaces
{
    public interface IApplicationService
    {
        IExpenseService ExpenseService { get; }
        ITravelService TravelService { get; }

        IActivityService ActivityService { get; }
        IMediaService MediaService { get; }
    }
}
