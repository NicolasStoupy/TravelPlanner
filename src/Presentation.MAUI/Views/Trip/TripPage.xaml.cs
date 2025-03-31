
using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI;

public partial class TripPage : ContentPage
{



    public TripPage(TripViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;


    }


}