using BussinessLogic.Entities;
using Commons;
using Commons.ErrorsHandlings;
using Microsoft.VisualBasic;
using Presentation.MAUI.Models;

namespace Presentation.MAUI.Interfaces
{
    public interface IAlertService
    {

        /// <summary>
        /// Displays a single alert message to the user.
        /// </summary>
        /// <param name="messageType">The type of message (e.g., Info, Warning, Error).</param>
        /// <param name="message">The message to display.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ShowAsync(MessageType messageType, string? message);

        /// <summary>
        /// Displays an alert dialog containing a list of messages, each separated by a new line.
        /// </summary>
        /// <param name="messageType">The type of message (e.g., Info, Warning, Error).</param>
        /// <param name="messages">The collection of message strings to display.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ShowAsync(MessageType messageType, IEnumerable<string?>? messages);       

        /// <summary>
        /// Displays the result to the user and conditionally calls the <see cref="Reset"/> method
        /// based on the success status of the result.
        /// </summary>       
        /// <param name="resetWhenResultIsSuccess">
        /// Determines when the <see cref="Reset"/> method should be called.
        /// If true (default), <see cref="Reset"/> is called only when the result is successful;
        /// if false, it's called when the result indicates failure.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task HandleResultAndResetAsync(IServiceResult result, BaseVM baseVM, bool resetWhenResultIsSuccess = true,bool showSuccess=true);

        /// <summary>
        /// Shows a confirmation dialog with two buttons and returns true if the user accepts.
        /// </summary>
        /// <param name="title">Dialog title (e.g. “Confirmation”).</param>
        /// <param name="message">Body text asking the user to confirm.</param>
        /// <param name="accept">Label for the affirmative button.</param>
        /// <param name="cancel">Label for the negative button.</param>
        /// <returns>True if the user tapped the accept button; otherwise false.</returns>
        Task<bool> ConfirmAsync(string title, string message, string accept = "Yes", string cancel = "No", params object?[]? args);
        Task ShowAsync(MessageType messageType,  string message, params object?[]? args);

        Task ShowAsync(IServiceResult result, bool showSuccess = false);
    }


}
