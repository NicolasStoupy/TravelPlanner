using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.EntityModels;
using Presentation.MAUI.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace Presentation.MAUI.ViewModel
{
    public partial class ActivitiesTravelVM : TravelVM
    {

        [ObservableProperty]
        private ObservableCollection<Infrastructure.EntityModels.Activity> _activities;

        [ObservableProperty]
        private bool _saveButtonVisible;

        public decimal TotalPlannedCost => 0;
        public decimal TotalRealCost =>0;
        public ActivitiesTravelVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
        }
     

        [RelayCommand]
        private void MoveUp(TravelActivity activity) { return; }

        [RelayCommand]
        private void MoveDown(TravelActivity activity) { return; }
        public async Task LoadData()
        {
            if (CurrentTravel == null)
            {
                await NoTravelSelected();
                return;
            }
            else
            {
                Activities = new ObservableCollection<Infrastructure.EntityModels.Activity>(
                                  _applicationService.ActivityService.GetActivities(CurrentTravel.Id));
                SaveButtonVisible = false;
            }
        }

        [RelayCommand]
        public async Task AddActivity() => await _navigationService.NavigateToNewActivity();
        public async Task OnAppearingAsync()
        {

            await LoadData();
        }

        [RelayCommand]
        public async Task SaveActivity()
        {
            await DisplayAlert(MAUI.Models.MessageType.Success, "OK");

        }

    }
}
