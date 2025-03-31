namespace BussinessLogic.Interfaces
{
    public interface IApplicationService
    {
        IExpenseService ExpenseService { get; }
        ITripService TripService { get; }
    }
}
