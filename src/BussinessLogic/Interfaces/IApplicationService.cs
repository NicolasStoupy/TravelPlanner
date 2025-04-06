namespace BussinessLogic.Interfaces
{
    public interface IApplicationService
    {
        IExpenseService ExpenseService { get; }
        ITravelService TravelService { get; }

        IMediaService MediaService { get; }
    }
}
