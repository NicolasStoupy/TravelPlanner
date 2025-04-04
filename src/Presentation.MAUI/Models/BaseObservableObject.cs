using CommunityToolkit.Mvvm.ComponentModel;


namespace Presentation.MAUI.Models
{
    public partial class BaseObservableObject : ObservableObject
    {
        [ObservableProperty]
        public bool isBusy;

        [ObservableProperty]
        public string title;
    }
}
