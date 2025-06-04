using BussinessLogic.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Interfaces;


namespace Presentation.MAUI.ViewModel.Activity
{
    [QueryProperty(nameof(ActivityID), "ActivityID")]

    public partial class NewActivityVM : ActivityVM
    {
        private readonly IUrlBuilder _urlBuilder;


        [ObservableProperty]
        private int _activityID;
        partial void OnActivityIDChanged(int value)
        {
            Mode = value != 0 ? Mode.Edit : Mode.New;
        }


        [ObservableProperty]
        private string? _currentUrl;

        partial void OnCurrentUrlChanged(string? value)
        {
            if (CurrentTravelActivity != null && value != null)
                CurrentTravelActivity.GoogleLink = value;
        }

        [ObservableProperty]
        private string? _activityName;

        partial void OnActivityNameChanged(string? value)
        {
            if (value is null || CurrentTravelActivity is null) return;

            CurrentTravelActivity.Name = value;
            CurrentUrl = _urlBuilder.BuildSearchUrl(value);
        }




        [ObservableProperty]
        private TravelActivity? _currentTravelActivity;

        partial void OnCurrentTravelActivityChanged(TravelActivity? value)
        {
            if (value is null)
                return;

            // 1) Initialiser le champ de texte
            ActivityName = value.Name;

            // 2) Initialiser l’URL
            CurrentUrl = !string.IsNullOrWhiteSpace(value.GoogleLink)
                ? value.GoogleLink
                : _urlBuilder.Url.BaseUrl;

            if (DataStore.ActivityType != null && value.ActivityType != null)
            {
                SelectedActivityType = DataStore.ActivityType
                    .FirstOrDefault(x => x.ID == value.ActivityType.ID);
            }
            else
            {
                SelectedActivityType = null;
            }
        }


        [ObservableProperty]
        private TypeOfActivity? _selectedActivityType;
        partial void OnSelectedActivityTypeChanged(TypeOfActivity? value)
        {
            if (value != null && CurrentTravelActivity != null)
                CurrentTravelActivity.ActivityType = value;
        }


        public NewActivityVM(IViewModelServices viewModelServices, IUrlBuilder urlBuilder) : base(viewModelServices)
        {
            _urlBuilder = urlBuilder;
            Reset();
        }
        [RelayCommand]
        public async Task Save()
        {
            var resultValidation = await _services.Validation.ValidateAndNotifyAsync(this);
            if (!resultValidation)
                return;
            if (CurrentTravel != null && CurrentTravelActivity != null)
            {
                if (Mode == Mode.Edit)
                {
                    var result = await _services.Application.ActivityService.UpdateActivity(CurrentTravelActivity, CurrentTravel.Id);

                    await _services.Alert.ShowAsync(result, true);
                    Mode = Mode.New;
                    Reset();

                    await _services.Navigation.GoBack();
                    return;

                }
                if (Mode == Mode.New)
                {
                    CurrentTravelActivity.TravelID = CurrentTravel.Id;
                    var result = await _services.Application.ActivityService.SaveNewActivity(CurrentTravelActivity);
                    await _services.Alert.ShowAsync(result, true);
                    if (result.IsSuccess)
                    {
                        Reset();
                        await _services.Navigation.GoBack();
                    }
                }
            }
            else
            {
                await NoTravelSelected();
                return;
            }
        }

        public override void Reset()
        {
            switch (Mode)
            {
                case Mode.New:
                    CurrentTravelActivity = new();
                    CurrentUrl = _urlBuilder.Url.BaseUrl;

                    return;

                case Mode.Edit:
                    var result = _services.Application.ActivityService.GetActivity(ActivityID);
                    if (result.IsSuccess)
                        CurrentTravelActivity = result.Value;
                    return;

                default:
                    return;

            }
        }
    }

}
