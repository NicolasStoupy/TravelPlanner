using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Presentation.MAUI.Services;
using System.Collections.ObjectModel;


namespace Presentation.MAUI.ViewModel.Activity
{
    [QueryProperty(nameof(ActivityID), "ActivityID")]
    public partial class NewCostActivityVM(INavigationService navigationService, IApplicationService applicationService) : TravelVM(navigationService, applicationService)
    {
        [ObservableProperty]
        private int _activityID;

        [ObservableProperty]
        private TravelActivity? _travelActivity;

        [ObservableProperty]
        private ObservableCollection<Cost>? _costs;

        [ObservableProperty]
        private string? _newCostName;
        [ObservableProperty]
        private double? _newCostAmount;
         [ObservableProperty]
        private string? _newCurrency;

        partial void OnActivityIDChanged(int oldValue, int newValue)
        {
            LoadData();

        }
        [RelayCommand]
        public async Task AddCost()
        {
            var cost = new Cost()
            {
                Currency = NewCurrency,
                Name = NewCostName,
                Price = NewCostAmount ?? 0
            };

           await DisplayAlert(_applicationService.ExpenseService.CreateCost(TravelActivity.ActivityID, cost));
            LoadData();
        }
        [RelayCommand]
        public async Task AddTicket(int costID)
        {
            if (CurrentTravel == null)
            {
                await NoTravelSelected();
                return;
            }
            var file = await LoadFile(FilePickerFileType.Images, "Telecharger votre ticket");
            if (file != null)
                await DisplayAlert(_applicationService.ExpenseService.SaveNewCost(CurrentTravel.Id, costID, file));
            LoadData();
        }

        [RelayCommand]
        public async Task RemoveTicket(Guid ticketId)
        {
            await DisplayAlert(_applicationService.ExpenseService.RemoveTicket(ticketId));
            LoadData();
        }
        [RelayCommand]
        public async Task RemoveCost(int costID)
        {
            await DisplayAlert(_applicationService.ExpenseService.RemoveCost(costID));
            LoadData();
        }

        [RelayCommand]
        public async Task OpenTicketAsync(Guid ticketId)
        {
            byte[]? content = _applicationService
                .MediaService
                .GetMedia(ticketId, Commons.TypeMedia.Images);
            await ShowFile(content);

        }

        public void LoadData()
        {
            NewCostAmount = 0;
            NewCostName = string.Empty;
            NewCurrency = string.Empty;
            TravelActivity = _applicationService.ActivityService.GetActivity(ActivityID);
            Costs = TravelActivity.Cost.ToObservableCollection();
        }
    }
}
