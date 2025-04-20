using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessLogic.Entities;

namespace Presentation.MAUI.Services
{
    public interface INavigationService
    {
        Task NavigateToNewTravelPageAsync();
        Task NavigateToTravelDetailsPageAsync(string travelID);
    

    }
}
