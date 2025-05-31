using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Models;
using Presentation.MAUI.Services;
using Presentation.MAUI.Validators;
using Presentation.MAUI.ViewModel;
using Presentation.MAUI.ViewModel.Activity;
using Serilog;

namespace Presentation.MAUI
{
    /// <summary>
    /// Provides extension methods to register presentation-layer dependencies in the DI container.
    /// </summary>
    public static class PresentationDependencies
    {
        /// <summary>
        /// Registers all presentation-layer services, view models, and validators.
        /// </summary>
        /// <param name="collection">The service collection to which dependencies will be added.</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddPresentation(this IServiceCollection collection)
        {
            // - Transient : always a new instance
            // - Scoped    : one instance per scope (e.g., per page)
            // - Singleton : a single shared instance

            collection.AddApplicationServices();
            collection.AddViewModels();
            collection.AddValidators();
            return collection;

         
        }

        /// <summary>
        /// Registers application-level services such as validation, alerts, and navigation.
        /// </summary>
        /// <param name="collection">The service collection to which application services will be added.</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection collection)
        {
            collection.AddSingleton<IValidationService, ValidationService>();
            collection.AddSingleton<IAlertService, AlertService>();
            collection.AddSingleton<INavigationService, NavigationService>();
            collection.AddSingleton<IFilePresentationService, FilePresentationService>();
            collection.AddSingleton<IOutputFileNameProvider, OutputFileNameProvider>();
            collection.AddSingleton<IViewModelServices, ViewModelServices>();
            collection.AddSingleton<IUrlBuilder, GoogleUrlBuilder>();
            return collection;
        }

        /// <summary>
        /// Registers view models with scoped lifetime.
        /// </summary>
        /// <param name="collection">The service collection to which view models will be added.</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddViewModels(this IServiceCollection collection)
        {
            collection.AddScoped<FinderTravelPageVM>();
            collection.AddScoped<NewTravelVM>();
            collection.AddScoped<NoteTravelVM>();
            collection.AddScoped<ActivitiesTravelVM>();
            collection.AddScoped<NewActivityVM>();
            collection.AddScoped<MemoriesTravelVM>();
            collection.AddScoped<NewCostActivityVM>();
            collection.AddScoped<ActivityFollowerVM>();
            collection.AddScoped<ImportExportVM>();
            return collection;
        }

        /// <summary>
        /// Registers FluentValidation validators for view models.
        /// </summary>
        /// <param name="collection">The service collection to which validators will be added.</param>
        /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddValidators(this IServiceCollection collection)
        {
            collection.AddScoped<IValidator<NewTravelVM>, NewTravelVMValidator>();
            collection.AddScoped<IValidator<NewActivityVM>, NewActivityVMValidator>();
            collection.AddScoped<IValidator<NoteTravelVM>, NoteTravelVMValidator>();
            collection.AddScoped<IValidator<NewCostActivityVM>, NewCostActivityVMValidator>();
            collection.AddScoped<IValidator<ActivityFollowerVM>, ActivityAttendeeVMValidators>();
            return collection;
        }

        /// <summary>
        /// Registers and configures the specified options types from the application's configuration.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="IServiceCollection"/> to add the configurations to.
        /// </param>
        /// <param name="builder">
        /// The <see cref="MauiAppBuilder"/> containing the application configuration.
        /// </param>
        /// <returns>
        /// The original <see cref="IServiceCollection"/> instance, enabling chaining.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the required configuration sections "OutputFileName" or "GoogleURL" are not found or invalid.
        /// </exception>
        public static IServiceCollection AddConfigurations(this IServiceCollection collection, MauiAppBuilder builder)
        {
            collection.Configure<FileNameSettings>(builder.Configuration.GetSection("FileNameSettings"));

            //injecte via Ioption dans le constructeur de FileNameSettings
            collection.Configure<FileNameSettings>(opts =>
            {
                opts.Patterns = builder.Configuration
                                     .GetSection("OutputFileName")
                                     .Get<Dictionary<string, string>>()
                                ?? throw new InvalidOperationException("Missing OutputFileName section");
            });

            //injecte via Ioption dans le constructeur de UrlBuilder
            collection.Configure<UrlBuilder>(opts =>
            {
                opts.BaseUrl = builder.Configuration
                                    .GetSection("GoogleURL")
                                    .Get<string>() ?? throw new InvalidOperationException("Missing GoogleURL section");
            });

       
            //Configuration du logger
            Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(builder.Configuration) // Charge la config
                        .CreateLogger();
            // 3) Brancher Serilog comme provider de logging
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(dispose: true);


            return collection;
        }

        /// <summary>
        /// Loads configuration settings from the "appsettings.json" file
        /// located in the application's current directory.
        /// </summary>
        /// <param name="manager">
        /// The <see cref="ConfigurationManager"/> to which the JSON file is added.
        /// </param>
        /// <returns>
        /// The <see cref="ConfigurationManager"/> instance with the JSON file loaded.
        /// </returns>
        public static ConfigurationManager LoadConfigurationsFile(this ConfigurationManager manager)
        {
            manager
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            return manager;
        }
    }
}