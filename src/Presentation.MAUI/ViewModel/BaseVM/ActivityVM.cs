using BussinessLogic.Entities;
using Commons;
using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.Interfaces;

namespace Presentation.MAUI.ViewModel
{
    public partial class ActivityVM : TravelVM
    {

        [ObservableProperty]
        private TravelActivity? _currentTravelActivity;

        public ActivityVM(IViewModelServices viewModelServices) : base(viewModelServices)
        {
        }

        /// <summary>
        /// Displays a warning alert indicating that no activity is selected and navigates the user to the travel activities page.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        protected async Task NoActivitySelected()
        {
            await _services.Alert.ShowAsync(MessageType.Warning, "Merci de sélectionner une activité avant d’ajouter un Participant.");

            await _services.Navigation.NavigateToTravelActivities();
        }
        /// <summary>
        /// Resets the ViewModel state by clearing the current travel activity.
        /// </summary>
        public override void Reset()
        {
            CurrentTravelActivity = default;
        }
    }
}
