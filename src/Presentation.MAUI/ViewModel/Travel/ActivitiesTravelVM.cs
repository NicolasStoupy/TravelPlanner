using BussinessLogic.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Resources.Localization;
using System.Collections.ObjectModel;

namespace Presentation.MAUI.ViewModel
{
    public partial class ActivitiesTravelVM(IViewModelServices viewModelServices) : ActivityVM(viewModelServices)
    {
        [ObservableProperty] private ObservableCollection<TravelActivity>? _activities;
        [ObservableProperty] private bool _saveButtonVisible;
        [ObservableProperty] private bool _modificationNotSaved;
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

        public async Task LoadData()
        {
            if (CurrentTravel == null)
            {
                await NoTravelSelected();
                return;
            }
            else
            {
                var result = await _services.Application.ActivityService.GetActivities(CurrentTravel.Id);
                if (result.Status.IsSuccess)
                {
                    Activities = new ObservableCollection<TravelActivity>(
                                     result.Value);
                    SaveButtonVisible = false;
                }


            }
        }
        [RelayCommand] public async Task SaveActivity() => await SaveSequence();
        [RelayCommand] public Task CostLinkClicked(int activityID) => _services.Navigation.NavigateToActivityCost(activityID);

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
        public async Task DeleteActivity(TravelActivity activity)
        {
            var attendeesQty = activity.Followers.Count();
            var activityCost = activity.Cost.Count();

            bool confirm = await _services.Alert
                .ConfirmAsync(DialogsStrings.DeleteActivity_Title,
                DialogsStrings.DeleteActivity_Confirmation,
                DialogsStrings.DeleteActivity_OK,
                DialogsStrings.DeleteActivity_NOK, attendeesQty, activityCost);

            if (!confirm)
                return;

            var result = await _services.Application.ActivityService.DeleteActivity(activity);
            await _services.Alert.ShowAsync(result.Status);
            if (result.Status.IsSuccess)
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
            if (Activities != null)
            {
                var result = await _services.Application.ActivityService.UpdateSequence(Activities.ToList());
                await _services.Alert.ShowAsync(result.Status);
                if (result.Status.IsSuccess)
                {
                    ModificationNotSaved = false;
                    await LoadData();
                }

                return;
            }


        }
    }
}