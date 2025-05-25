using Presentation.MAUI.Models;
using Presentation.MAUI.ViewModel.Activity;
using System.Threading.Tasks;

namespace Presentation.MAUI.Views.Activity;

public partial class NewActivityPage : ContentPage
{
    public NewActivityPage(NewActivityVM vM)
    {
        InitializeComponent();
        BindingContext = vM;
    }   

    protected override void OnAppearing()
    {
        try
        {
            base.OnAppearing();

            if (BindingContext is NewActivityVM vm)
            {
               
                vm.Reset();
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }
        return;

    }
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is NewActivityVM vm)
        {
            vm.ActivityID = 0;
            vm.Reset();
        }
    }

}