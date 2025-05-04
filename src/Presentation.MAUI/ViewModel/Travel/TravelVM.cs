using BussinessLogic.Interfaces;
using Presentation.MAUI.Models;
using Presentation.MAUI.Services;
using BussinessLogic.Entities;
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Presentation.MAUI.ViewModel
{
    public partial class TravelVM : BaseVM
    {
        protected static Travel? CurrentTravel { get; set; }
        public string ModeDisplay => Mode.ToDisplayName();

        partial void OnModeChanged(Mode value)
        {
            OnPropertyChanged(nameof(ModeDisplay));
        }

        [ObservableProperty]
        private Mode _mode;


        [ObservableProperty]

        private List<TypeOfActivity> _activityType;

        [ObservableProperty]
        private List<string> _currencies;

        public TravelVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
            _activityType = _applicationService.ActivityService.GetActivitiesTypes();
            _currencies = _applicationService.ExpenseService.GetCurrencies();
        }
        protected async Task NoTravelSelected()
        {
            await DisplayAlert(MessageType.Warning, "Merci de sélectionner un voyage avant d’ajouter une note.");
            await _navigationService.NavigateToTravelFinder();
        }
        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
