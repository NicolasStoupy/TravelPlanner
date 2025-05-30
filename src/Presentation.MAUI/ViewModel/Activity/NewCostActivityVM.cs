using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Resources.Localization;
using System.Collections.ObjectModel;


namespace Presentation.MAUI.ViewModel.Activity
{
    [QueryProperty(nameof(ActivityID), "ActivityID")]
    public partial class NewCostActivityVM(IViewModelServices viewModelServices) : TravelVM(viewModelServices)
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
            if (!await _services.Validation.ValidateAndNotifyAsync(this))
                return;
            else
            {
                var cost = new Cost()
                {
                    Currency = NewCurrency,
                    Name = NewCostName,
                    Price = NewCostAmount ?? 0
                };

                await _services.Alert.ShowAsync(
                    await _services.Application.ExpenseService.CreateCostAsync(TravelActivity.ActivityID, cost)
                    );
                LoadData();
            }
        }
        [RelayCommand]
        public async Task AddTicket(int costID)
        {
            if (CurrentTravel == null)
            {
                await NoTravelSelected();
                return;
            }
            var file = await _services.DialogFile.LoadFileAsync(FilePickerFileType.Images, "Telecharger votre ticket");
            if (file != null)
                await _services.Alert.ShowAsync(
                    await _services.Application.ExpenseService.SaveNewCostAsync(CurrentTravel.Id, costID, file));
            LoadData();
        }

        [RelayCommand]
        public async Task RemoveTicket(Guid ticketId)
        {
            await _services.Alert.ShowAsync( 
                await _services.Application.ExpenseService.RemoveTicketAsync(ticketId));
            LoadData();
        }
        [RelayCommand]
        public async Task RemoveCost(int costID)
        {
            var countTicketList = Costs?.FirstOrDefault(c => c.CostID == costID)?.TicketsList.Count;
            if (countTicketList != null)
            {
                bool confirm = await _services.Alert
                    .ConfirmAsync(DialogsStrings.DeleteExpense_Title,
                    DialogsStrings.DeleteExpense_Confirmation,
                    DialogsStrings.DeleteExpense_OK,
                    DialogsStrings.DeleteExpense_NOK,
                    countTicketList);
                if (confirm)
                {
                    await _services.Alert.ShowAsync(
                        await _services.Application.ExpenseService.RemoveCostAsync(costID));
                    LoadData();
                }
            }
        }

        [RelayCommand]
        public async Task OpenTicketAsync(Guid ticketId)
        {
            var serviceResult = _services.Application
                .MediaService
                .GetMedia(ticketId, Commons.TypeMedia.Images);
            if (serviceResult.IsSuccess)
            {

                await _services.DialogFile.ShowFileAsync(serviceResult.Value);
                return;
            }
           await _services.Alert.ShowAsync(serviceResult);
        }

        public void LoadData()
        {
            NewCostAmount = 0;
            NewCostName = string.Empty;
            NewCurrency = string.Empty;
            var result = _services.Application.ActivityService.GetActivity(ActivityID);
            if (result.IsSuccess)
            {
                TravelActivity = result.Value;
                Costs = TravelActivity.Cost.ToObservableCollection();

            }

        }
    }
}
