using BussinessLogic.Interfaces;
using Presentation.MAUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.ViewModel
{
    public class MemoriesTravelVM : TravelVM
    {
        public MemoriesTravelVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
        }
    }
}
