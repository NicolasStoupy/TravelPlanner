
using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views.Travel;

public partial class MemoriesTravelPage : ContentPage
{
    public MemoriesTravelPage(MemoriesTravelVM vm)
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

        if (BindingContext is MemoriesTravelVM vm)
            vm.Reset();
        return;
    }
    /// <summary>
    /// Triggered when a checkbox is (un)checked in the memory file list.
    /// Updates the ExtraAction flag based on current selection.
    /// </summary>
    /// <param name="sender">The CheckBox that triggered the event.</param>
    /// <param name="e">CheckedChangedEventArgs containing the new value.</param>
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (BindingContext is MemoriesTravelVM vm)
        {
            vm.ExtraAction = vm.ExtraActionIsEnabled();
        }

    }
}