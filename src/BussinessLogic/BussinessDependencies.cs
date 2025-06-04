using BussinessLogic.Interfaces;
using BussinessLogic.Mappings;
using BussinessLogic.Mappings.Resolvers;
using BussinessLogic.Services;
using Infrastructure.Documents;
using Microsoft.Extensions.DependencyInjection;

namespace BussinessLogic
{
    /// <summary>
    /// Registers business services, resolvers, and AutoMapper profiles into the IServiceCollection.
    /// </summary>
    /// <param name="collection">The IServiceCollection to add services to.</param>
    /// <returns>The updated IServiceCollection for chaining.</returns>
    public static class BussinessDependencies
    {
        /// <summary>
        /// Registers business services, resolvers, and AutoMapper profiles into the IServiceCollection.
        /// </summary>
        /// <param name="collection">The IServiceCollection to add services to.</param>
        /// <returns>The updated IServiceCollection for chaining.</returns>
        public static IServiceCollection AddBussiness(this IServiceCollection collection)
        {
            collection.AddScoped<IExpenseService, ExpenseService>();
            collection.AddScoped<ITravelService, TravelService>();
            collection.AddScoped<IMediaService, MediaService>();
            collection.AddScoped<IActivityService, ActivityService>();
            collection.AddScoped<ILogBookService, LogBookService>();

            collection.AddScoped<DocumentProvider>();
            collection.AddScoped<TravelImageResolver>();
            collection.AddScoped<TravelNotesResolver>();
            collection.AddScoped<IApplicationService, ApplicationService>();

            collection.AddAutoMapper(typeof(MappingProfiles).Assembly);

            return collection;
        }
    }
}