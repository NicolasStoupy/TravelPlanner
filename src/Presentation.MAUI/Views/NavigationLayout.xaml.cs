using Presentation.MAUI.Services;

namespace Presentation.MAUI.Views;

public partial class NavigationLayout : ContentView
{
    
    public NavigationLayout()
    {
        InitializeComponent();
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        if (Shell.Current.Navigation.NavigationStack.Count > 1)
        {
            await Shell.Current.GoToAsync("..");

        }

    }
}