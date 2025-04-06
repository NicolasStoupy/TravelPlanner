using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class TravelPage : ContentPage
{

	public TravelPage(TravelPageViewModel vm)
	{
		
		
		InitializeComponent();
		BindingContext = vm;
    }
    
}