using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class TravelPage : ContentPage
{

	public TravelPage(TravelPageViewModel vm)
	{
		
		
		InitializeComponent();
		BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TravelPageViewModel vm)
        {
            vm.Reset();
        }
    }
}