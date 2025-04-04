using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Trip;

public partial class MyTravelPage : ContentPage
{

	public MyTravelPage(TripMainPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    
}