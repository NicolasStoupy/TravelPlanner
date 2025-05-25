using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.ViewModel
{
    public partial class ActivityVM : TravelVM
    {

        [ObservableProperty]
        private TravelActivity? _currentTravelActivity;

        public ActivityVM(IViewModelServices viewModelServices) : base(viewModelServices)
        {
        }
        protected async Task NoActivitySelected()
        {
            await _services.Alert.ShowAsync(MessageType.Warning, "Merci de sélectionner une activité avant d’ajouter un Participant.");

            await _services.Navigation.NavigateToTravelActivities();
        }
        public override void Reset()
        {
            CurrentTravelActivity = default;
        }
    }
}
