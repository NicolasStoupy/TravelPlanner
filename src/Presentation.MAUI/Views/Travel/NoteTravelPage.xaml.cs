using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class NoteTravelPage : ContentPage
{
    public NoteTravelPage(NoteTravelVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is NoteTravelVM vm)
        {
            vm.Reset();
        }
    }
}