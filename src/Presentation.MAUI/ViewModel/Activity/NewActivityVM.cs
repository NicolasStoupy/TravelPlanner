using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Presentation.MAUI.Services;

namespace Presentation.MAUI.ViewModel.Activity
{
    public partial class NewActivityVM : TravelVM
    {
        [ObservableProperty]
        private TravelActivity _currentTravelActivity;
        
        [ObservableProperty]
        private TypeOfActivity? _selectedActivityType;
        partial void OnSelectedActivityTypeChanged(TypeOfActivity? value)
        {
            if (value != null && CurrentTravelActivity != null)
                CurrentTravelActivity.ActivityType = value; // ou autre champ d'identifiant
        }
        protected override IValidator? GetValidator() => new NewActivityVMValidator();
        public NewActivityVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {

            CurrentTravelActivity = new();
        }

        [RelayCommand]
        public async Task Save()
        {
            if (CurrentTravel != null && CurrentTravelActivity != null)
            {
                if (Mode == Mode.Edit) { }
                if (Mode == Mode.New)
                {
                    CurrentTravelActivity.TravelID = CurrentTravel.Id;
                    var result = await _applicationService.ActivityService.SaveNewActivity(CurrentTravelActivity);
                    await DisplayAlert(result);
                    if (result.IsSuccess)
                    {
                        CurrentTravelActivity = new(); // Réinitialiser
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
    }
}
