using Presentation.MAUI.ViewModel.Activity;

namespace Presentation.MAUI.Views.Activity;

public partial class NewCostActivityPage : ContentPage
{
	public NewCostActivityPage(NewCostActivityVM vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}