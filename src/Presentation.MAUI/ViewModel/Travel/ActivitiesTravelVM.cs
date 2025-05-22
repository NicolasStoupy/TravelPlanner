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
        [ObservableProperty]
        private bool _modificationNotSaved;
        partial void OnModificationNotSavedChanged(bool value)
        {
            if (value)
            {
                SaveButtonVisible = true;
            }
            else
            {
                SaveButtonVisible = false;
            }

        }

        public decimal TotalPlannedCost => 0;
        public decimal TotalRealCost => 0;
        [ObservableProperty]

        private UrlWebViewSource url = new UrlWebViewSource { Url = "https://www.google.com/search?q=Paris&hl=fr&udm=2" };
        public ActivitiesTravelVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
        }

        [RelayCommand]
        private void MoveUp(TravelActivity activity)
        { return; }

        [RelayCommand]
        private void MoveDown(TravelActivity activity)
        { return; }

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
        public async Task AddActivity()
        {
            await PendingChange();
            await _navigationService.NavigateToNewActivity();

        }

        async Task PendingChange()
        {
            if (ModificationNotSaved)
            {
                bool confirm = await Shell.Current.DisplayAlert("Modifications en attente",
                    "Vous avez des changements non enregistrés. Voulez-vous les enregistrer avant de quitter ?",
                    "Enregistrer", "Quitter sans enregistrer");
                if (confirm)
                {
                    await SaveSequence();

                }


            }
        }
        [RelayCommand]
        public async Task EditActivity(TravelActivity travelActivity)
        {
            await PendingChange();
            await _navigationService.NavigateToEditActivity(travelActivity);
        }

        public async Task OnAppearingAsync()
        {
            await LoadData();
        }

        [RelayCommand]
        public async Task SaveActivity()
        {
            await SaveSequence();
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

        [RelayCommand]
        public async Task OpenGoogleLink(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return;

            // Vérifie que c'est bien une URI valide
            if (Uri.TryCreate(link, UriKind.Absolute, out var uri))
            {
                await Launcher.OpenAsync(uri);
            }
        }


        public async Task SaveSequence()
        {
            await _applicationService.ActivityService.UpdateSequence(Activities);
            ModificationNotSaved = false;
            await LoadData();
            return;
        }

        [RelayCommand]
        public Task CostLinkClicked(int activityID) => _navigationService.NavigateToActivityCost(activityID);
        

       


    }
}