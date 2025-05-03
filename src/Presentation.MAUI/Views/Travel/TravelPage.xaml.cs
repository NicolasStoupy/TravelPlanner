using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class TravelPage : ContentPage
{
    public TravelPage(TravelPageViewModel vm)
    {
        try
        {
            InitializeComponent();
            BindingContext = vm;
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }

    }
    protected override void OnAppearing()
    {
        try
        {
            base.OnAppearing();

            if (BindingContext is TravelPageViewModel vm)
            {
                vm.Reset();
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }

    }
}