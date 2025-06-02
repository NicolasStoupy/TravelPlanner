
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure
{
    public static class InfrastructureDependencies
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection collection, IConfiguration configuration)
        {
            // Register a DbContext factory for TravelPlannerContext, enabling lazy loading proxies
            // and configuring SQL Server using the "DbConnection" connection string.
            collection.AddDbContextFactory<TravelPlannerContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DbConnection"))
            );

            collection.AddScoped<DocumentProvider>();


            return collection;
        }
    }
}
