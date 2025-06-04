namespace Presentation.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            AppRouting.RegisterRoutes();
        }

       
    }
}