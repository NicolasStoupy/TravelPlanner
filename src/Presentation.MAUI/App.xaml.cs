using System.Runtime.ExceptionServices;

namespace Presentation.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            InitializeComponent();


        }

        private void CurrentDomain_FirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"********************************** UNHANDLED EXCEPTION! Details: {e.Exception.ToString()}");
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}