
namespace Presentation.MAUI.Views;

public partial class FullScreenImagePage : ContentPage
{
    public FullScreenImagePage(byte[] imageBytes)
    {
        InitializeComponent();
        ImageView.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
    }
}