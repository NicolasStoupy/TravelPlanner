using BussinessLogic.Interfaces;
using Presentation.MAUI.Models;
using BussinessLogic.Entities;
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.Interfaces;

namespace Presentation.MAUI.ViewModel
{
    public partial class TravelVM : BaseVM
    {

        protected static Travel? CurrentTravel { get; set; }
        protected async Task NoTravelSelected()
        {
            await  _services.Alert.ShowAsync(MessageType.Warning, "Merci de sélectionner un voyage avant d’ajouter une note.");
         
            await _services.Navigation.NavigateToTravelFinder();
        }
        public TravelVM(IViewModelServices viewModelServices) : base(viewModelServices)
        {

        }

        public override void Reset()
        {
            return;
        }
    }
}
