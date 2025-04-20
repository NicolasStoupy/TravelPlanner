using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class NotePage : ContentPage
{
    public NotePage(TravelNotePageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is TravelNotePageViewModel vm)
        {
            vm.Reset();
        }
    }
}