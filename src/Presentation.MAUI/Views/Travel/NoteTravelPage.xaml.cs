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
            RulesToBeApply(vm);
        }

        return;
    }
    protected override  void OnDisappearing()
    {
        base.OnDisappearing();

        if (BindingContext is NoteTravelVM vm)
        {
            RulesToBeApply(vm);
        }


        return;

    }

    private  void RulesToBeApply(NoteTravelVM vm)
    {
        vm.NavigationVisible = AppShell.Current.Navigation.NavigationStack.Count > 1;
        vm.NoteTo = vm.NavigationVisible == true ? NoteTo.Activity : NoteTo.Travel;
        vm.Reset();
    }
}