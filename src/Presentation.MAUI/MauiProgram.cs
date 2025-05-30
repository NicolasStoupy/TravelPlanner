
using Autofac;
using Autofac.Extras.DynamicProxy;
using BussinessLogic;

using BussinessLogic.Services;
using CommunityToolkit.Maui;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using QuestPDF.Infrastructure;
using Serilog;


namespace Presentation.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
      
        builder.Configuration.LoadConfigurationsFile();     
        builder.UseMauiCommunityToolkit();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddBussiness();
        builder.Services.AddPresentation();
        builder.Services.AddConfigurations(builder);
    
        QuestPDF.Settings.License = LicenseType.Community;

#pragma warning disable CA1416 // Valider la compatibilité de la plateforme
        builder
            .UseMauiApp<App>()
          
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .ConfigureContainer(new Autofac.Extensions.DependencyInjection.AutofacServiceProviderFactory(),
            containerBuilder =>
            {
                // IMPORTANT : Les services IExpenseService, etc. sont déjà
                // ajoutés au IServiceCollection (via AddBusiness)               

                // Création d'un intercepteur pour le logging
                containerBuilder.RegisterType<LoggingInterceptor>().AsSelf(); ;

                // Activation de l’interception pour tous les services se terminant par "Service"
                containerBuilder
                    .RegisterAssemblyTypes(typeof(ExpenseService).Assembly)
                    .Where(t => t.Name.EndsWith("Service"))
                    .AsImplementedInterfaces()
                    .EnableInterfaceInterceptors()
                    .InterceptedBy(typeof(LoggingInterceptor));
            }
            
        );
#pragma warning restore CA1416 // Valider la compatibilité de la plateforme


#if DEBUG

        builder.Logging.AddDebug();
        

#endif



        return builder.Build();
    }
}
