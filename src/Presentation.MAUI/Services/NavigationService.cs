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
            await Shell.Current.GoToAsync($"//NewTravelPage");
        }

        public async Task NavigateToTravelDetailsPageAsync(TravelItem travelItem)
        {
            
            await Shell.Current.GoToAsync($"//InformationPage?travelItemId={travelItem.Id}");
        }
    }
}
