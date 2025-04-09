using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.Services;

namespace Presentation.MAUI.Models
{
    /// <summary>
    /// Provides a base class for ViewModels in the MAUI application.
    /// Includes support for validation, busy state, title management, and navigation helpers.
    /// </summary>
    public abstract partial class BaseViewModel : ObservableValidator
    {
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
        protected readonly IApplicationService _applicationService;

        protected readonly INavigationService _navigationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The navigation service used for page transitions.</param>
        /// <param name="applicationService">The application service providing access to business logic.</param>
        public BaseViewModel(INavigationService navigationService, IApplicationService applicationService)
        {
            _navigationService = navigationService;
            _applicationService = applicationService;
        }

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
            // Clear previous errors
            ClearErrors();

            // Run validation on all properties
            ValidateAllProperties();

            // Check for validation errors
            if (HasErrors)
            {
                var messages = GetErrors()
                    .Select(e => e.ErrorMessage)
                    .Distinct();

                await BaseViewModel.DisplayAlerts(MessageType.Warning, messages);
                return false;
            }

            // All properties are valid
            return true;
        }
    }
}