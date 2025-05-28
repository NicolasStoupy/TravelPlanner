using BussinessLogic.Entities;
using Commons.Models;

namespace BussinessLogic.Interfaces
{
    public interface ILogBookService
    {
        Task<Result> DeleteNote(Note note);
        Task<Result> AddNote(Note? note, int travelID);

        // Writting

        Task<Result> EditNote(Note note);
    }
}