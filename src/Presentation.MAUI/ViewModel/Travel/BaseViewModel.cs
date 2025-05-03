using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation;
using Presentation.MAUI.Services;
using System.Threading.Tasks;

namespace Presentation.MAUI.Models
{
    /// <summary>
    /// Provides a base class for ViewModels in the MAUI application.
    /// Includes support for validation, busy state, title management, and navigation helpers.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
    /// </remarks>
    /// <param name="navigationService">The navigation service used for page transitions.</param>
    /// <param name="applicationService">The application service providing access to business logic.</param>
    public abstract partial class BaseViewModel(INavigationService navigationService, IApplicationService applicationService) : ObservableValidator
    {
        protected virtual IValidator? GetValidator() => null;

        protected static Travel? CurrentTravel { get; set; }

        /// <summary>
        /// Indicates whether the ViewModel is performing a background operation.
        /// Typically used to show or hide loading indicators in the UI.
        /// </summary>
        [ObservableProperty]
        public bool isBusy = false;

        /// <summary>
        /// The title of the current view, often displayed in the page header.
        /// </summary>
        [ObservableProperty]
        public string title = string.Empty;

        /// <summary>
        /// Provides access to navigation functionality within the application.
        /// </summary>

        /// <summary>
        /// Provides access to application-level services (business logic, persistence, etc.).
        /// </summary>
        protected readonly IApplicationService _applicationService = applicationService;

        protected readonly INavigationService _navigationService = navigationService;

        /// <summary>
        /// Resets the state of the ViewModel to its initial/default values.
        /// Must be implemented by derived classes.
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Displays a single alert message to the user.
        /// </summary>
        /// <param name="messageType">The type of message (e.g., Info, Warning, Error).</param>
        /// <param name="messages">The message to display.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected static async Task DisplayAlert(MessageType messageType, string? messages)
        {
            if (messages != null)
            {
                await Shell.Current.DisplayAlert(messageType.ToString(), messages, "OK");
            }
        }

        /// <summary>
        /// Displays an alert dialog containing a list of messages, each separated by a new line.
        /// </summary>
        /// <param name="messageType">The type of message (e.g., Info, Warning, Error).</param>
        /// <param name="messages">The collection of message strings to display.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected static async Task DisplayAlerts(MessageType messageType, IEnumerable<string?>? messages)
        {
            if (messages != null)
            {
                await Shell.Current.DisplayAlert(messageType.ToString(), string.Join("\n", messages), "OK");
            }
        }

        protected static async Task DisplayAlert(Commons.Models.Result result)
        {
            if (result != null)
            {
                var messageType = result.IsSuccess ? MessageType.Success : MessageType.Error;

                await Shell.Current.DisplayAlert(messageType.ToString(), result.Message, "OK");
            }
        }

        /// <summary>
        /// Displays the result to the user and conditionally calls the <see cref="Reset"/> method
        /// based on the success status of the result.
        /// </summary>
        /// <param name="result">The <see cref="Commons.Models.Result"/> object to display and evaluate.</param>
        /// <param name="resetWhenResultIsSuccess">
        /// Determines when the <see cref="Reset"/> method should be called.
        /// If true (default), <see cref="Reset"/> is called only when the result is successful;
        /// if false, it's called when the result indicates failure.
        /// </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task HandleResultAndReset(Commons.Models.Result result, bool resetWhenResultIsSuccess = true)
        {
            await DisplayAlert(result);
            if (result != null && result.IsSuccess == resetWhenResultIsSuccess)
            {
                Reset();
            }
        }

        /// <summary>
        /// Validates all properties in the ViewModel using Data Annotations (e.g., [Required], [Range], etc.).
        /// Displays an alert with the validation errors if any are found.
        /// </summary>
        /// <returns>
        /// A <see cref="Task{Boolean}"/> that returns <c>true</c> if all properties are valid,
        /// or <c>false</c> if any validation errors are present (and shown to the user).
        /// </returns>
        /// <remarks>
        /// This method first clears any previous validation errors using <see cref="ClearErrors"/>,
        /// then calls <see cref="ValidateAllProperties"/> to trigger validation for all observable properties.
        /// </remarks>
        protected async Task<bool> ValidateAll()
        {
            // Récupérer le validateur FluentValidation associé
            var validator = GetValidator() ?? throw new InvalidOperationException("Aucun validateur FluentValidation n'a été fourni.");

            // Validate the model 
            var result = await validator.ValidateAsync(new ValidationContext<object>(this));

            //// Nettoyer les erreurs précédentes (si tu utilises un système d’erreurs custom)
            //ClearErrors();

            // S'il y a des erreurs, les afficher
            if (!result.IsValid)
            {
                var messages = result.Errors
                    .Select(e => e.ErrorMessage)
                    .Distinct();

                await DisplayAlerts(MessageType.Warning, messages);
                return false;
            }

            return true;
        }
    }
}