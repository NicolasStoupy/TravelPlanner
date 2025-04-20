using BussinessLogic.Interfaces;
using BussinessLogic.Mappings;
using BussinessLogic.Mappings.Resolvers;
using BussinessLogic.Services;
using Infrastructure.Documents;
using Microsoft.Extensions.DependencyInjection;

namespace BussinessLogic
{
    public static class BussinessDependencies
    {

        public static IServiceCollection AddBussiness(this IServiceCollection collection)
        {

            collection.AddScoped<IExpenseService, ExpenseService>();
            collection.AddScoped<ITravelService, TravelService>();
            collection.AddScoped<IMediaService, MediaService>();
            collection.AddScoped<DocumentProvider>();
           collection.AddScoped<TravelImageResolver>();
            collection.AddScoped<TravelNotesResolver>();
            collection.AddScoped<IApplicationService, ApplicationService>();
 
            collection.AddAutoMapper(typeof(MappingProfiles).Assembly);
      
           
            return collection;
        }
    }
}
