
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
            collection.AddDbContextFactory<TravelPlannerContext>(options =>
            //options.UseLazyLoadingProxies()
            options.UseSqlServer(configuration.GetConnectionString("DbConnection")));// Scoped pour une instance par requête

            collection.AddScoped<DocumentProvider>();


            return collection;
        }
    }
}
