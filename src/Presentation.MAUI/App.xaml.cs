using Microsoft.Extensions.Logging;

namespace Presentation.MAUI
{
    public partial class App : Application
    {
        private readonly ILogger<App> _logger;
        public App(ILogger<App> logger)
        {
            //First‐chance : toutes les exceptions dès qu’elles sont levées
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomain_FirstChanceException;

            //UnhandledException : uniquement quand aucune couche n’attrape l’exception
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            //Tâches asynchrones non observées
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

            InitializeComponent();
            _logger = logger;
        
        }

        // Si une Task n’est jamais awaitée et qu’elle lève une exception, cet événement la capte
        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            _logger.LogCritical(e.Exception, "TaskScheduler_UnobservedTaskException");
        }

        // Cet événement NE se déclenche que si l’exception n’a pas été capturée par un catch
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Log unhandled exceptions
            if (e.ExceptionObject is Exception ex)
            {
                _logger.LogCritical("Unhandled exception:", ex);
            }
            else
            {
                _logger.LogCritical("Unhandled non-exception object:", e.ExceptionObject);
            }

            Application.Current?.Quit();

        }


        // Premier “filtre” : dès qu’une exception est levée (même si un catch existe ensuite)
        private void CurrentDomain_FirstChanceException(object? sender, System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs e)
        {
            //Filtrer les exceptions WinRT internes levées par ThrowExceptionForHR
            if (e.Exception is InvalidOperationException invOp
                && invOp.StackTrace?.Contains("WinRT.ExceptionHelpers") == true)
            {
                // On ignore cette exception car MAUI/WinRT la gère en interne
                return;
            }

            // 2. Logguer la vraie exception
            try
            {
                _logger.LogInformation(e.Exception, "FirstChanceException");
            }
            catch
            {
                // Si le log échoue, ne pas bloquer l’application
            }

        }



        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }



    }
}