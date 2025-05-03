namespace Presentation.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
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
