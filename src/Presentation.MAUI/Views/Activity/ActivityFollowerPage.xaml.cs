using Presentation.MAUI.ViewModel.Activity;

namespace Presentation.MAUI.Views.Activity;

public partial class ActivityFollowerPage : ContentPage
{
	public ActivityFollowerPage(ActivityFollowerVM vM)
	{
		BindingContext = vM;
		InitializeComponent();
	}

    protected override void OnDisappearing()
	{
		base.OnDisappearing();
        if (BindingContext is ActivityFollowerVM vm)
        {

            vm.Reset();
        }
    }
}