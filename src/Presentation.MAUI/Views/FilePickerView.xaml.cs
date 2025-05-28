using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Presentation.MAUI.Views;

/// <summary>
/// A view that allows users to pick multiple image files, displays them,
/// and optionally send the collected files via a bound command.
/// </summary>
public partial class FilePickerView : ContentView
{
    /// <summary>
    /// Backing store for the <see cref="Files"/> bindable property.
    /// Contains the binary data of the selected files.
    /// </summary>
    public static readonly BindableProperty FilesProperty =
        BindableProperty.Create(
            nameof(Files),
            typeof(ObservableCollection<byte[]>),
            typeof(FilePickerView),
            default(ObservableCollection<byte[]>),
            BindingMode.TwoWay);

    /// <summary>
    /// Gets or sets the collection of file data selected by the user.
    /// This property can be bound in XAML or code-behind for two-way updates.
    /// </summary>
    public ObservableCollection<byte[]> Files
    {
        get => (ObservableCollection<byte[]>)GetValue(FilesProperty);
        set => SetValue(FilesProperty, value);
    }
    /// <summary>
    /// Backing store for the <see cref="SendFilesCommand"/> bindable property.
    /// Allows injection of a command to handle sending the selected files.
    /// </summary>
    public static readonly BindableProperty SendFilesCommandProperty =
        BindableProperty.Create(
            nameof(SendFilesCommand),
            typeof(ICommand),
            typeof(FilePickerView),
            null);

    /// <summary>
    /// Gets or sets the command to execute when the user taps the Send button.
    /// The command is passed the <see cref="Files"/> collection as its parameter.
    /// </summary>
    public ICommand? SendFilesCommand
    {
        get => (ICommand?)GetValue(SendFilesCommandProperty);
        set => SetValue(SendFilesCommandProperty, value);

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FilePickerView"/> class.
    /// </summary>
    public FilePickerView() => InitializeComponent();

    /// <summary>
    /// Handles the click event of the Pick File button.
    /// Opens the system file picker to select multiple images,
    /// reads their bytes, and adds them to the <see cref="Files"/> collection.
    /// </summary>
    private async void OnPickFileClicked(object sender, EventArgs e)
    {

        var results = await FilePicker.PickMultipleAsync(new PickOptions
        {
            PickerTitle = "Choisir des éléments",
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

        return;
    }

    /// <summary>
    /// Handles the click event of the delete button for each image.
    /// Removes the corresponding image byte array from the <see cref="Files"/> collection.
    /// </summary>
    private void OnDeleteImageClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is byte[] imageToRemove)
        {
            Files?.Remove(imageToRemove);
        }
        SetBtnValidationVisibility(); return;
    }
    /// <summary>
    /// Toggles the visibility of the validation/send button
    /// based on whether there are any files in the <see cref="Files"/> collection.
    /// </summary>
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
        return;
    }

    /// <summary>
    /// Invokes the bound <see cref="SendFilesCommand"/>, passing the <see cref="Files"/> collection,
    /// then clears the collection and updates the send button visibility.
    /// </summary>
    private void Send()
    {
        if (SendFilesCommand?.CanExecute(Files) == true)
        {
            SendFilesCommand.Execute(Files);
            Files.Clear();

        }
        SetBtnValidationVisibility();
        return;
    }

    /// <summary>
    /// Handles the click event of the send button in the UI.
    /// Calls the <see cref="Send"/> method.
    /// </summary>
    private void OnSendClicked(object sender, EventArgs e) => Send();

}