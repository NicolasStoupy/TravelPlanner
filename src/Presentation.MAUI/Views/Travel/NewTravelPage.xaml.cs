using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class NewTravelPage : ContentPage
{
	public NewTravelPage( NewTravelPageViewModel vm)
	{
		InitializeComponent();

		this.BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

      
    }
}