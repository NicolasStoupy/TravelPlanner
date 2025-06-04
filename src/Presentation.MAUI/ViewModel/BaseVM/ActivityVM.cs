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
        protected async Task NoActivitySelected()
        {
            await _services.Alert.ShowAsync(MessageType.Warning, "Merci de sélectionner une activité avant d’ajouter un Participant.");

            await _services.Navigation.NavigateToTravelActivities();
        }
        public override void Reset()
        {
            CurrentTravelActivity = default;
        }
    }
}
