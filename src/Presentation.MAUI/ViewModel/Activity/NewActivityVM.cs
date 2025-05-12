using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Presentation.MAUI.Services;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Presentation.MAUI.ViewModel.Activity
{
    [QueryProperty(nameof(ActivityID), "ActivityID")]
    public partial class NewActivityVM : TravelVM
    {
        [ObservableProperty]
        private int _activityID;
        partial void OnActivityIDChanged(int value)
        {
            Mode = Mode.Edit;
        }

        const string BASE_URL = "https://www.google.com/maps/search/";

        [ObservableProperty]
        private string _currentUrl;

        partial void OnCurrentUrlChanged(string value)
        {
            if (CurrentTravelActivity != null)
                CurrentTravelActivity.GoogleLink = value;
        }

        [ObservableProperty]
        private string _activityName;

        partial void OnActivityNameChanged(string value)
        {
            CurrentTravelActivity.Name = value;
            if (!string.IsNullOrWhiteSpace(value))
            {
                CurrentUrl = BASE_URL + string.Join("+", value.Split(' '));
            }
        }

        [ObservableProperty]
        private TravelActivity _currentTravelActivity;

        partial void OnCurrentTravelActivityChanged(TravelActivity value)
        {
            if (value is not null)
            {
                ActivityName = value.Name;

                if (value is INotifyPropertyChanged npc)
                {
                    npc.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == nameof(TravelActivity.Name))
                        {
                            ActivityName = value.Name;
                        }
                    };
                }
            }
        }

        [ObservableProperty]
        private TypeOfActivity? _selectedActivityType;

        partial void OnSelectedActivityTypeChanged(TypeOfActivity? value)
        {
            if (value != null && CurrentTravelActivity != null)
                CurrentTravelActivity.ActivityType = value;
        }

        protected override IValidator? GetValidator() => new NewActivityVMValidator();

        public NewActivityVM(INavigationService navigationService,
                             IApplicationService applicationService
                            ) : base(navigationService, applicationService)
        {
            Reset();
        }

        [RelayCommand]
        public async Task Save()
        {
            if (CurrentTravel != null && CurrentTravelActivity != null)
            {
                if (Mode == Mode.Edit)
                {
                    var result = _applicationService.ActivityService.UpdateActivity(CurrentTravelActivity,CurrentTravel.Id);

                    await DisplayAlert(await result);

                }
                if (Mode == Mode.New)
                {
                    CurrentTravelActivity.TravelID = CurrentTravel.Id;
                    var result = await _applicationService.ActivityService.SaveNewActivity(CurrentTravelActivity);
                    await DisplayAlert(result);
                    if (result.IsSuccess)
                    {
                        CurrentTravelActivity = new();
                        await Shell.Current.GoToAsync("..");
                    }
                }
            }
            else
            {
                await NoTravelSelected();
                return;
            }
        }

        public override async void Reset()
        {
            switch (Mode)
            {
                case Mode.New:
                    CurrentTravelActivity = new();
                    CurrentUrl = BASE_URL;

                    return;

                case Mode.Edit:
                    CurrentTravelActivity = await _applicationService.ActivityService.GetActivity(ActivityID);

                    return;

                default:
                    return;

            }
        }
    }

}
