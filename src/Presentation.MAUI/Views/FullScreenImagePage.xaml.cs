namespace Presentation.MAUI.Views;

/// <summary>
/// A <see cref="ContentPage"/> that displays a single image in full-screen mode.
/// </summary>
public partial class FullScreenImagePage : ContentPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FullScreenImagePage"/> class
    /// and sets the image to display from the provided byte array.
    /// </summary>
    /// <param name="imageBytes">
    /// A byte array containing the image data to display full-screen.
    /// </param>
    public FullScreenImagePage(byte[] imageBytes)
    {
        InitializeComponent();
        ImageView.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
    }
}