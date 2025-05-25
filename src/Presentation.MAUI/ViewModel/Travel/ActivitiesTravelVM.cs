using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.EntityModels;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Resources.Localization;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Presentation.MAUI.ViewModel
{
    public partial class ActivitiesTravelVM : ActivityVM
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
        //[ObservableProperty]

        //private UrlWebViewSource url = new UrlWebViewSource { Url = "https://www.google.com/search?q=Paris&hl=fr&udm=2" };
        public ActivitiesTravelVM(IViewModelServices viewModelServices) : base(viewModelServices)
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
                                  _services.Application.ActivityService.GetActivities(CurrentTravel.Id));
                SaveButtonVisible = false;
            }
        }

        [RelayCommand]
        public async Task AddActivity()
        {
            await PendingChange();
            await _services.Navigation.NavigateToNewActivity();
        }

        async Task PendingChange()
        {
            if (ModificationNotSaved)
            {
                bool confirm = await _services.Alert.ConfirmAsync(
                    DialogsStrings.WIP_Title,
                    DialogsStrings.WIP_Confirmation,
                    DialogsStrings.WIP_OK,
                    DialogsStrings.WIP_NOK);
               
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
            await _services.Navigation.NavigateToEditActivity(travelActivity);
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

            bool confirm = await _services.Alert
                .ConfirmAsync(DialogsStrings.DeleteActivity_Title,
                DialogsStrings.DeleteActivity_Confirmation,
                DialogsStrings.DeleteActivity_OK,
                DialogsStrings.DeleteActivity_NOK, attendeesQty,activityCost );           
            if (!confirm)
                return;
            await _services.Alert.ShowAsync(await _services.Application.ActivityService.DeleteActivity(activity));
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

        [RelayCommand]
        public async Task Follower(int activityID)
        {
            await _services.Navigation.NavigateToActivityFollower(activityID);
        }
        public async Task SaveSequence()
        {
            await _services.Application.ActivityService.UpdateSequence(Activities);
            ModificationNotSaved = false;
            await LoadData();
            return;
        }

        [RelayCommand]
        public Task CostLinkClicked(int activityID) => _services.Navigation.NavigateToActivityCost(activityID);
        

       


    }
}