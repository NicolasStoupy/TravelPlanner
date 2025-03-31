using BussinessLogic;
using BussinessLogic.DTOs;
using BussinessLogic.Interfaces;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation.ConsoleApp
{
    internal class Program
    {




        private static void Main(string[] args)
        {
            IConfiguration config = LoadConfiguration();
            ServiceProvider serviceProvider = ConfigureServices(config);
            using IServiceScope scope = serviceProvider.CreateScope();
            ITripService tripService = scope.ServiceProvider.GetRequiredService<ITripService>();

            foreach (TripDTO trip in tripService.GetTrips())
            {
                Console.WriteLine($"{trip.Name}:{trip.NumberPeople}");
            }

            TripDTO newTrip = new()
            {
                Name = "Mon Voyage en France ",
                Description = "Mon super Voyage en france",
                NumberPeople = 10,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(20),
                Budget = 1000,
                CurrencyCode = "EUR"
            };

            _ = tripService.CreateTrip(newTrip);



        }

        private static ServiceProvider ConfigureServices(IConfiguration configuration)
        {
            ServiceCollection services = new();

            _ = services.AddInfrastructure(configuration);
            _ = services.AddBussiness();
            return services.BuildServiceProvider();
        }
        private static IConfiguration LoadConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
