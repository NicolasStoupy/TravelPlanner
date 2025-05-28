using Presentation.MAUI.Models;
using BussinessLogic.Entities;
using Presentation.MAUI.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Commons;

namespace Presentation.MAUI.ViewModel
{
    public partial class TravelVM : BaseVM
    {

        [ObservableProperty]
        protected static Travel? _currentTravel;
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
