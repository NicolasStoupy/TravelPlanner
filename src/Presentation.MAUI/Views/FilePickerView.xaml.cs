using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Presentation.MAUI.Views;

public partial class FilePickerView : ContentView
{

    public static readonly BindableProperty FilesProperty =
        BindableProperty.Create(
            nameof(Files),
            typeof(ObservableCollection<byte[]>),
            typeof(FilePickerView),
            default(ObservableCollection<byte[]>),
            BindingMode.TwoWay);

    public ObservableCollection<byte[]> Files
    {
        get => (ObservableCollection<byte[]>)GetValue(FilesProperty);
        set => SetValue(FilesProperty, value);
    }
    // Déclaration de la BindableProperty
    public static readonly BindableProperty SendFilesCommandProperty =
        BindableProperty.Create(
            nameof(SendFilesCommand),
            typeof(ICommand),
            typeof(FilePickerView),
            null);

    public ICommand? SendFilesCommand
    {
        get => (ICommand?)GetValue(SendFilesCommandProperty);
        set => SetValue(SendFilesCommandProperty, value);

    }
    public FilePickerView() => InitializeComponent();

    private async void OnPickFileClicked(object sender, EventArgs e)
    {

        var results = await FilePicker.PickMultipleAsync(new PickOptions
        {
            PickerTitle = "Choisir des images",
            FileTypes = FilePickerFileType.Images
        });

        if (results != null)
        {
            Files ??= new ObservableCollection<byte[]>();

            foreach (var file in results)
            {
                using var stream = await file.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                Files.Add(ms.ToArray());
            }
        }
        SetBtnValidationVisibility();


    }
    private void OnDeleteImageClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is byte[] imageToRemove)
        {
            Files?.Remove(imageToRemove);
        }
        SetBtnValidationVisibility();
    }

    private void SetBtnValidationVisibility()
    {
        if (Files.Count > 0)
        {
            BtnValidation.IsVisible = true;
        }
        else
        {
            BtnValidation.IsVisible = false;
        }
    }

    private void Send()
    {
        if (SendFilesCommand?.CanExecute(Files) == true)
        {
            SendFilesCommand.Execute(Files);
            Files.Clear();

        }
        SetBtnValidationVisibility();
    }

    // Exemple bouton lié à cette méthode
    private void OnSendClicked(object sender, EventArgs e) => Send();

}