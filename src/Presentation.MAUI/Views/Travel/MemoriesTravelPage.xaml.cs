using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class MemoriesTravelPage : ContentPage
{
	public MemoriesTravelPage(MemoriesTravelVM vm)
	{
		InitializeComponent();
        this.BindingContext = vm;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is MemoriesTravelVM vm)
            vm.Reset();
        return;
    }
}