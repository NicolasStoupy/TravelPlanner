using Commons.Resources;

namespace Commons.ErrorsHandlings
{
    /// <summary>
    /// Represents the result of a service operation, including success status, returned value, and message details.
    /// </summary>
    /// <typeparam name="T">The type of the value returned on a successful operation.</typeparam>
    public class ServiceResult<T> : IServiceResult
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the value returned by the service when the operation succeeds.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Gets a message describing the result of the operation, such as success or error details.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the type of message (e.g., Success, Warning, Error).
        /// </summary>
        public MessageType MessageType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResult{T}"/> class.
        /// </summary>
        /// <param name="value">The value returned by the operation.</param>
        /// <param name="success">A boolean indicating if the operation was successful.</param>
        /// <param name="messageType">The type of message associated with the operation result.</param>
        /// <param name="message">An optional message describing the result.</param>
        private ServiceResult(T value, bool success, MessageType messageType, string message = "")
        {
            MessageType = messageType;
            Value = value;
            IsSuccess = success;
            Message = message;
        }

        /// <summary>
        /// Creates a successful service result with the specified value and optional message.
        /// </summary>
        /// <param name="value">The value returned by the successful operation.</param>
        /// <param name="mess">An optional custom success message. If not provided, a default success message is used.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> representing a successful operation.</returns>
        public static ServiceResult<T> Success(T value, string mess = "")
            => new(value, true, messageType: MessageType.Success, message: string.IsNullOrEmpty(mess)
                  ? GlobalServiceMessage.SUCCESS : mess);

        /// <summary>
        /// Creates a warning service result with a default value and the provided warning message.
        /// </summary>
        /// <param name="errorWarning">The warning message to include.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> representing a warning state.</returns>
        public static ServiceResult<T> Warning(string errorWarning)
           => new(default, true, messageType: MessageType.Warning);

        /// <summary>
        /// Creates a failure service result with a default value and the provided error message.
        /// </summary>
        /// <param name="error">The error message describing why the operation failed.</param>
        /// <returns>A <see cref="ServiceResult{T}"/> representing a failed operation.</returns>
        public static ServiceResult<T> Failure(string error)
            => new(default, false, MessageType.Error, error);

        /// <summary>
        /// Placeholder for a failure service result with an invalid travel file object.
        /// </summary>
        /// <param name="iNVALID_TRAVEL_FILE">An object representing an invalid travel file. Not implemented.</param>
        /// <returns>Throws <see cref="NotImplementedException"/>.</returns>
        public static ServiceResult<bool> Failure(object iNVALID_TRAVEL_FILE)
        {
            throw new NotImplementedException();
        }
    }
}