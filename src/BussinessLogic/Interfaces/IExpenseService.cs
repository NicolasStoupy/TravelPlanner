using BussinessLogic.Entities;
using Commons.Models;

namespace BussinessLogic.Interfaces
{
    public interface IExpenseService
    {
        List<string> GetCurrencies();

        List<Cost> GetCost(int ActivityID);
        Result SaveNewCost(int id, int activityID, byte[] file);
        Result RemoveTicket(Guid ticketId);
        Result RemoveCost(int costID);
        Result CreateCost( int activityID, Cost newCost);
    }
}
