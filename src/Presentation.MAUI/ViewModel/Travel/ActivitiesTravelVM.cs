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
        private ObservableCollection<TravelActivity>? _activities;

        [ObservableProperty]
        private bool _saveButtonVisible;

        public decimal TotalPlannedCost => 0;
        public decimal TotalRealCost => 0;
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
                Activities = new ObservableCollection<TravelActivity>(
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
        [RelayCommand]
        public async Task DeleteActivity(TravelActivity activity)
        {
            var attendeesQty = activity.Followers.Count();
            var activityCost = activity.Cost.Count();

            bool confirm = await Shell.Current.DisplayAlert(
                             "Confirmation de suppression",
                             $"Voulez-vous vraiment supprimer cette activité ?\n\n" +
                             $"Cette activité contient :\n" +
                             $"- {attendeesQty} participant(s)\n" +
                             $"- {activityCost} facture(s)\n\n" +
                             $" Tous ces éléments seront également supprimés de façon définitive.",
                             "Oui, supprimer",
                             "Annuler");

            if (!confirm)
                return;
          
            await DisplayAlert(await _applicationService.ActivityService.DeleteActivity(activity));
            await LoadData();
        }
    }
}
