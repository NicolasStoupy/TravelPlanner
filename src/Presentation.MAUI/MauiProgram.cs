
using BussinessLogic;
using CommunityToolkit.Maui;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QuestPDF.Infrastructure;


namespace Presentation.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        builder.UseMauiCommunityToolkit();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddBussiness();
        builder.Services.AddPresentation();

        QuestPDF.Settings.License = LicenseType.Community;

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


#if DEBUG
       
        builder.Logging.AddDebug();

#endif



        return builder.Build();
    }
}
