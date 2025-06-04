using Presentation.MAUI.Models;
using BussinessLogic.Entities;
using Presentation.MAUI.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Commons;

namespace Presentation.MAUI.ViewModel
{
    /// <summary>
    /// ViewModel for managing the current travel context, including date boundaries and selection handling.
    /// </summary>
    public partial class TravelVM(IViewModelServices viewModelServices) : BaseVM(viewModelServices)
    {
        /// <summary>
        /// Gets or sets the currently selected travel. Null if no travel is selected.
        /// </summary>
        [ObservableProperty]
        protected static Travel? _currentTravel;
        /// <summary>
        /// Gets or sets the minimum allowed date for activities based on the selected travel's start date.
        /// </summary>
        [ObservableProperty]
        protected static DateTime _minDate;
        /// <summary>
        /// Gets or sets the maximum allowed date for activities based on the selected travel's end date.
        /// </summary>
        [ObservableProperty]
        protected static DateTime _maxDate;
        /// <summary>
        /// Called when the CurrentTravel property changes. Sets the mode and updates MinDate and MaxDate accordingly.
        /// </summary>
        /// <param name="value">The newly selected travel, or null if deselected.</param>
        partial void OnCurrentTravelChanged(Travel? value)
        {
            if (value == null) { 
                Mode= Mode.New;            
                        
            }
            Mode = Mode.Edit;

            MinDate = value?.StartDate?? DateTime.Now;
            MaxDate = value?.EndDate ?? DateTime.Now;
        }
        /// <summary>
        /// Displays a warning indicating that no travel is selected and navigates to the travel finder view.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task NoTravelSelected()
        {
            await  _services.Alert.ShowAsync(MessageType.Warning, "Merci de sélectionner un voyage avant d’ajouter une note.");
         
            await _services.Navigation.NavigateToTravelFinder();
        }

        /// <summary>
        /// Resets the ViewModel state. This implementation does nothing.
        /// </summary>
        public override void Reset()
        {
            return;
        }
        /// <summary>
        /// Asynchronously resets the ViewModel state. This implementation completes immediately.
        /// </summary>
        /// <returns>A completed <see cref="Task"/>.</returns>
        public override Task ResetAsync()
        {
            return Task.CompletedTask;
        }
    }
}
