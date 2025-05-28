using Presentation.MAUI.Services;

namespace Presentation.MAUI.Views;

/// <summary>
/// A layout containing a back button that navigates to the previous page when clicked.
/// </summary>
public partial class NavigationLayout : ContentView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationLayout"/> class.
    /// </summary>
    public NavigationLayout()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handles the Clicked event of the back button. If there is more than one
    /// page on the navigation stack, navigates back to the previous page.
    /// </summary>
    /// <param name="sender">The object that raised the event (back button).</param>
    /// <param name="e">Event arguments for the click event.</param>
    private async void OnBackClicked(object sender, EventArgs e)
    {
        if (Shell.Current.Navigation.NavigationStack.Count > 1)
        {
            await Shell.Current.GoToAsync("..");
        }
        return;
    }
}