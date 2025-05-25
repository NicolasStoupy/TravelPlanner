
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.ViewModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Presentation.MAUI.Models
{
    /// <summary>
    /// Provides a base class for ViewModels in the MAUI application.
    /// Includes support for validation, busy state, title management, and navigation helpers.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="BaseVM"/> class.
    /// </remarks>
    /// <param name="navigationService">The navigation service used for page transitions.</param>
    /// <param name="applicationService">The application service providing access to business logic.</param>
    public abstract partial class BaseVM(IViewModelServices viewModelServices) : ObservableValidator
    {       

        protected readonly IViewModelServices _services =  viewModelServices;

        #region ViewBehavior

       
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
        /// Gets the display name of the current mode.
        /// </summary>
        public string ModeDisplay => Mode.ToDisplayName();

        /// <summary>
        /// Gets or sets the current mode. Can be Edit or New 
        /// </summary>
        [ObservableProperty]
        private Mode _mode;
        partial void OnModeChanged(Mode value)
        {
            OnPropertyChanged(nameof(ModeDisplay));
            return;
        }
        /// <summary>
        /// Resets the state of the ViewModel to its initial/default values.
        /// Must be implemented by derived classes.
        /// </summary>
        public abstract void Reset();

        #endregion

        [ObservableProperty]
        protected CommonsDataStore _dataStore = new CommonsDataStore(viewModelServices.Application);
    }
}