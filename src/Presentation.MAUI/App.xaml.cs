using System.Runtime.ExceptionServices;

namespace Presentation.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
          
        }

        private void CurrentDomain_FirstChanceException(object? sender,
   System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            try
            {
                // logguer
                Console.WriteLine(e.Exception.Message);
            }
            catch
            {
                // HACK : l'échec du log de l'exception ne doit pas être bloquante
            }
        }


        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
           
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

    }
}