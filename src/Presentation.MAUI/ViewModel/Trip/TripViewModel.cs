using BussinessLogic.DTOs;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace Presentation.MAUI.ViewModel
{
    public partial class TripViewModel : ObservableObject
    {
        private readonly IApplicationService _applicationService;
        [ObservableProperty]
        private bool busy = true;

        public ObservableCollection<TripDTO> Trips { get; set; } = [];

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="applicationService"></param>
        public TripViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
            UpdateData();

        }

        /// <summary>
        /// Updates the data by retrieving trips from the TripService and assigning them to the 'trips' variable.
        /// </summary>
        private void UpdateData()
        {
            Busy = true;
            Trips.Clear();
            foreach (TripDTO trip in _applicationService.TripService.GetTrips())
            {
                Trips.Add(trip);
            }

            Busy = false;
        }

        /// <summary>
        /// Command for refresh button
        /// </summary>
        [RelayCommand]
        public void Refresh()
        {
            UpdateData();
        }
    }
}