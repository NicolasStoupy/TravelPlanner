using BussinessLogic.Entities;
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Interfaces;
using System.Collections.ObjectModel;


namespace Presentation.MAUI.ViewModel.Activity
{
    [QueryProperty(nameof(ActivityID), "ActivityID")]
    public partial class ActivityFollowerVM : ActivityVM
    {

        [ObservableProperty]
        private int _activityID;
        partial void OnActivityIDChanged(int value)
        {
            CurrentTravelActivity = _services.Application.ActivityService.GetActivity(_activityID);
        }
        public ActivityFollowerVM(IViewModelServices viewModelServices) : base(viewModelServices)
        {
        }

        [ObservableProperty]
        private ObservableCollection<Follower>? _followerList;


        [ObservableProperty]
        private TravelActivity? _currentTravelActivity;


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
        partial void OnCurrentTravelActivityChanged(TravelActivity? value)
        {
            if (value != null)
            {
                FollowerList = value.Followers.ToObservableCollection<Follower>();
            }

        }

        [RelayCommand]
        public async Task AddFollower()
        {

            if (CurrentTravelActivity == null) { await NoActivitySelected(); return; }

            if (await _services.Validation.ValidateAndNotifyAsync(this))
            {
                await _services.Application.ActivityService.AddFollower(CurrentTravelActivity.ActivityID, newFollower);
                OnActivityIDChanged(CurrentTravelActivity.ActivityID);
            }
          
        }
        [RelayCommand]

        public async Task RemoveFollower(Follower follower)
        {
            if(follower == null || CurrentTravelActivity == null) return;

            await _services.Application.ActivityService.RemoveFollower(follower,CurrentTravelActivity.ActivityID);
            OnActivityIDChanged(CurrentTravelActivity.ActivityID);
        }
        public override void Reset()
        {
            Name = null;
            Forname = null;
            newFollower = new Follower();           

        }


    }
}
