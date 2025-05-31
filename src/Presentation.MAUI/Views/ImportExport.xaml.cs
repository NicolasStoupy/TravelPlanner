using Presentation.MAUI.ViewModel;

namespace Presentation.MAUI.Views;

public partial class ImportExport : ContentPage
{
    public ImportExport(ImportExportVM vM)
    {
        BindingContext = vM;
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ImportExportVM vm)
        {
            await vm.ResetAsync();
        }
        return;
    }


}
