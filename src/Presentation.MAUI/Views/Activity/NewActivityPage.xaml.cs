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

    private async void  WebView_Navigated(object sender, WebNavigatedEventArgs e)
    {       
        var webview = (WebView)sender;

        var text = await  webview.EvaluateJavaScriptAsync("document.body.innerText");
      
        
        return;
    }

    private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        var webview = (WebView)sender;
        var text = webview.EvaluateJavaScriptAsync("document.body.innerText");
        return;
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
}