using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.Models;
using Presentation.MAUI.Services;


namespace Presentation.MAUI.ViewModel
{

    public partial class TravelNotePageViewModel: BaseViewModel
    {
        [ObservableProperty] Travel? _travels;

        public TravelNotePageViewModel(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
            loadData();
          
        }

        public void loadData()
        {
            if (CurrentTravel != null && CurrentTravel.Id !=0) { 
             Travels = _applicationService.TravelService.GetTravel(CurrentTravel.Id);
            }
            else
            {
               
            }
        }

        public override void Reset()
        {
              
            
        }
    }
}
