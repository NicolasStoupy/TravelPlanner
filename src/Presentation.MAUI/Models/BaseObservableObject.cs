using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.Services;


namespace Presentation.MAUI.Models
{
    public partial class BaseObservableObject : ObservableObject
    {
        [ObservableProperty]
        public bool isBusy=false;

        [ObservableProperty]
        public string title= string.Empty;


        protected readonly INavigationService _navigationService;


        public BaseObservableObject(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }
    }
}
