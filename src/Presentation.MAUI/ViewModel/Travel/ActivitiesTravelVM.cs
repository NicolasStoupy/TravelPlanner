using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.EntityModels;
using Presentation.MAUI.Services;
using System.Collections.ObjectModel;


namespace Presentation.MAUI.ViewModel
{
    public partial class ActivitiesTravelVM : TravelVM
    {
        [ObservableProperty]
        private ObservableCollection<TravelActivity> _activities;

        public ActivitiesTravelVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
            LoadData();
        }

        public void LoadData()
        {
            _activities = new ObservableCollection<TravelActivity>(CurrentTravel?.TravelActivities);

        }

        [RelayCommand]
        public async Task AddActivity() => await _navigationService.NavigateToNewActivity();


    }
}
