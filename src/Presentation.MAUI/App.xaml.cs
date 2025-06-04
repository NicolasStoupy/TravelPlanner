

using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace Presentation.MAUI
{
    public partial class App : Application
    {
        private readonly ILogger<App> _logger;
        public App(ILogger<App> logger)
        {
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;
    
            InitializeComponent();
            _logger = logger;
         
        }

      

        private void CurrentDomain_FirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
        {
            try
            {
                _logger.LogError(e.Exception, "FirstChanceException");
            }
            catch
            {
                // Si le log échoue, on ne bloque pas l'application
            }
        }



        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

    }
}