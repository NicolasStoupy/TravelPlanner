namespace BussinessLogic.Interfaces
{
    /// <summary>
    /// Defines methods to access core application domain services such as expenses, travel management, activities, media operations, and logbook access.
    /// </summary>
    public interface IApplicationService
    {
        /// <summary>
        /// Gets the expense-related service for creating, removing, and querying costs, tickets, and currencies.
        /// </summary>
        IExpenseService ExpenseService { get; }

        /// <summary>
        /// Gets the travel service for exporting, importing, and managing travel entities.
        /// </summary>
        ITravelService TravelService { get; }

        /// <summary>
        /// Gets the activity service for adding, updating, and managing travel activities.
        /// </summary>
        IActivityService ActivityService { get; }

        /// <summary>
        /// Gets the media service for retrieving, saving, and deleting media files (images, PDFs) associated with travels and activities.
        /// </summary>
        IMediaService MediaService { get; }

        /// <summary>
        /// Gets the logbook service for writing and retrieving application logs or notes.
        /// </summary>
        ILogBookService LogBookService { get; }
    }

}
