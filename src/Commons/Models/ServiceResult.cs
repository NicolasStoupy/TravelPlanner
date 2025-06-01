using Commons.ErrorsHandlings;
using Commons.Resources;

namespace Commons.Models
{
    public class ServiceResult<T> : IServiceResult
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Message { get; }
        public MessageType MessageType { get; }


        private ServiceResult(T value, bool success, MessageType messageType, string message = "")
        {
            MessageType = messageType;
            Value = value;
            IsSuccess = success;
            Message = message;
        }

        public static ServiceResult<T> Success(T value, string mess = "")
            => new(value, true, messageType: MessageType.Success, message: string.IsNullOrEmpty(mess)
                  ? GlobalServiceMessage.SUCCESS : mess);

        public static ServiceResult<T> Warning(string errorWarning)
           => new(default, true, messageType: MessageType.Warning);
        public static ServiceResult<T> Failure(string error)
            => new(default, false, MessageType.Error, error);

        public static ServiceResult<bool> Failure(object iNVALID_TRAVEL_FILE)
        {
            throw new NotImplementedException();
        }
    }
}
