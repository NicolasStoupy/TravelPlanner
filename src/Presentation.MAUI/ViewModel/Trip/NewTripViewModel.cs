
using BussinessLogic;
using BussinessLogic.DTOs;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;



namespace Presentation.MAUI.ViewModel
{
    public partial class NewTripViewModel : ObservableObject
    {
        private readonly IApplicationService _applicationService;

        [ObservableProperty]

        private TripDTO trip = new();

        [ObservableProperty]
        private List<string> currencies = [];


        public NewTripViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
            currencies.Clear();
            currencies = _applicationService.ExpenseService.GetCurrencies();
        }





        [RelayCommand]
        public async Task SaveTrip()
        {
           ExecutionStatus  result = await _applicationService.TripService.CreateTrip(Trip);

            if(result == ExecutionStatus.Success)
            {
                Console.WriteLine(result);
            }
        }

    }
}
