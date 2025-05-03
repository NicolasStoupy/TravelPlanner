using Presentation.MAUI.ViewModel.Activity;

namespace Presentation.MAUI.Views.Activity;

public partial class NewActivityPage : ContentPage
{
	public NewActivityPage(NewActivityVM vM)
	{
		InitializeComponent();
		BindingContext=vM;
	}
}