using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessLogic.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.MAUI.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

       

        public async Task NavigateToNewTravelPageAsync()
        {
            await Shell.Current.GoToAsync($"//TravelInformations");
        }

        public async Task NavigateToNewTravel(string travelID)
        {
            
            await Shell.Current.GoToAsync($"//TravelInformations?travelID={travelID}");
        }

        public async Task NavigateToTravelFinder()
        {
            await Shell.Current.GoToAsync("//TravelFinder");
        }

        public async Task NavigateToNewActivity()
        {
            await Shell.Current.GoToAsync("ActivityNew");
        }

        public async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
