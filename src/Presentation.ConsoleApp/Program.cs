using BussinessLogic;
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
            ITravelService tripService = scope.ServiceProvider.GetRequiredService<ITravelService>();

          



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
