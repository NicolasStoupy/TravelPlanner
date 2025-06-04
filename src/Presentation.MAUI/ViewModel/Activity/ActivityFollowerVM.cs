using BussinessLogic.Entities;
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Interfaces;
using System.Collections.ObjectModel;


namespace Presentation.MAUI.ViewModel.Activity
{
    [QueryProperty(nameof(ActivityID), "ActivityID")]
    public partial class ActivityFollowerVM(IViewModelServices viewModelServices) : ActivityVM(viewModelServices)
    {

        [ObservableProperty]
        private int _activityID;
        partial void OnActivityIDChanged(int value)
        {
            var result = _services.Application.ActivityService.GetActivity(_activityID);
            if (result.IsSuccess)
            {
                CurrentTravelActivity = result.Value;
            }
        }

        [ObservableProperty]
        private ObservableCollection<Follower>? _followerList;


        [ObservableProperty]
        private TravelActivity? _currentTravelActivity;
        partial void OnCurrentTravelActivityChanged(TravelActivity? value)
        {
            if (value != null)
            {
                FollowerList = value.Followers.ToObservableCollection<Follower>();
            }

        }

        private Follower newFollower = new Follower();

        [ObservableProperty]
        private string? _name;

        partial void OnNameChanged(string? value)
        {
            if (value != null)
                newFollower.Name = value;

        }

        [ObservableProperty]
        private string? _forname;
        partial void OnFornameChanged(string? value)
        {
            if (value != null)
                newFollower.LastName = value;

        }

        /// <summary>
        /// Adds a follower to the currently selected travel activity.
        /// </summary>
        /// <remarks>
        /// If no activity is selected, displays a notification. Otherwise, validates and notifies.
        /// On successful addition, displays the result and refreshes the activity view.
        /// </remarks>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task AddFollower()
        {

            if (CurrentTravelActivity == null) { await NoActivitySelected(); return; }

            if (await _services.Validation.ValidateAndNotifyAsync(this))
            {
                var result = await _services.Application.ActivityService.AddFollower(CurrentTravelActivity.ActivityID, newFollower);
                await _services.Alert.ShowAsync(result);
                if (result.IsSuccess)
                {
                    OnActivityIDChanged(CurrentTravelActivity.ActivityID);
                }

            }
        }
        /// <summary>
        /// Removes a follower from the specified travel activity.
        /// </summary>
        /// <param name="follower">The follower to remove.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RelayCommand]
        public async Task RemoveFollower(Follower follower)
        {
            if (follower == null || CurrentTravelActivity == null) return;

            var result = await _services.Application.ActivityService.RemoveFollower(follower, CurrentTravelActivity.ActivityID);
            await _services.Alert.ShowAsync(result);
            if (result.IsSuccess) { OnActivityIDChanged(CurrentTravelActivity.ActivityID); }

        }
        public override void Reset()
        {
            Name = null;
            Forname = null;
            newFollower = new Follower();

        }


    }
}
