using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class NewTravelPage : ContentPage
{
	public NewTravelPage( NewTravelVM vm)
	{
		InitializeComponent();

		this.BindingContext = vm;
	}

    /// <summary>
    /// Called when the page appears. Resets the ViewModel to refresh the data.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is NewTravelVM vm)
        {
            if (vm.Mode == Mode.New)
            {
                vm.Reset();
            }
        }
      
        return;
    }


}