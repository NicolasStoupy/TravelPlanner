namespace Commons
{
    /// <summary>
    /// Enumeration representing the type of media file.
    /// </summary>
    public enum TypeMedia
    {
        /// <summary>
        /// No media type specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// Images media type.
        /// </summary>
        Images = 1,

        /// <summary>
        /// PDF media type.
        /// </summary>
        Pdf = 2
    }

    /// <summary>
    /// Enumeration representing the type of message for alerts or notifications.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Indicates a successful operation.
        /// </summary>
        Success,

        /// <summary>
        /// Indicates an error occurred.
        /// </summary>
        Error,

        /// <summary>
        /// Indicates a warning message.
        /// </summary>
        Warning
    }
}