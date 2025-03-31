using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI;

public partial class NewTripPage : ContentPage
{
    public NewTripPage(NewTripViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}