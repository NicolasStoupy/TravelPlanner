using Presentation.MAUI.Views.Activity;

namespace Presentation.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            AppRouting.RegisterRoutes();
           
        }
        public void SetShellBackground(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return;

            // Convert byte[] to ImageSource
            ImageSource imageSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));

            // Apply to Image
           
         
        }
    }
}
