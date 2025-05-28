namespace Commons.ErrorsHandlings
{
    public interface IServiceResult
    {
        bool IsSuccess { get; }
        MessageType MessageType { get; }

        string Message { get; }
    }
}