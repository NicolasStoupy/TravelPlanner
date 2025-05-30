

using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Runtime.ExceptionServices;

namespace Presentation.MAUI
{
    public partial class App : Application
    {
        private readonly ILogger<App> _logger;
        public App(ILogger<App> logger)
        {
            _logger = logger;
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
           
        }

        private void CurrentDomain_FirstChanceException(object? sender,FirstChanceExceptionEventArgs e)
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