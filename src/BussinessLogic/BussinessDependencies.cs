using BussinessLogic.Interfaces;
using BussinessLogic.Mappings;
using BussinessLogic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BussinessLogic
{
    public static class BussinessDependencies
    {

        public static IServiceCollection AddBussiness(this IServiceCollection collection)
        {

            collection.AddScoped<IExpenseService, ExpenseService>();
            collection.AddScoped<ITripService, TripService>();
            collection.AddScoped<IApplicationService, ApplicationService>();
            collection.AddAutoMapper(typeof(MappingProfile));
            return collection;
        }
    }
}
