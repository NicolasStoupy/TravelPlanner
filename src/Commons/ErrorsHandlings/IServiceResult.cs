namespace Commons.ErrorsHandlings
{
    /// <summary>
    /// Defines a standard result contract for service operations, including status and message.
    /// </summary>
    public interface IServiceResult
    {
        /// <summary>
        /// Gets a value indicating whether the service operation was successful.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// Gets the type of message associated with the service result (e.g., Success, Warning, Error).
        /// </summary>
        MessageType MessageType { get; }

        /// <summary>
        /// Gets a descriptive message providing additional details about the service result.
        /// </summary>
        string Message { get; }
    }
}