using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class ActivitiesTravelPage : ContentPage
{
	public ActivitiesTravelPage(ActivitiesTravelVM vm)
	{
		
		InitializeComponent();
		BindingContext = vm;
	}
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ActivitiesTravelVM vm)
            await vm.OnAppearingAsync();
    }
}