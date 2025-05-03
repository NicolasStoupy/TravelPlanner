using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Presentation.MAUI.Services;


namespace Presentation.MAUI.ViewModel.Activity
{
    public partial class NewActivityVM : TravelVM
    {
        [ObservableProperty]
        private Infrastructure.EntityModels.Activity _travelActivity;



        protected override IValidator? GetValidator() => new NewActivityVMValidator();
        public NewActivityVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {

            TravelActivity = new();
        }

        [RelayCommand]
        public async Task Save()
        {
            if (CurrentTravel != null)
            {
                if (Mode == Mode.Edit) { }
                if (Mode == Mode.New)
                {
                    var result = await _applicationService.ActivityService.SaveNewActivity(CurrentTravel.Id, TravelActivity);
                    await DisplayAlert(result);
                    if (result.IsSuccess)
                    {
                        TravelActivity = new(); // Réinitialiser
                        await Shell.Current.GoToAsync("..");
                    }
                }

            }
            else
            {
                await NoTravelSelected();
            }
        }
    }
}
