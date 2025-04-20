using Presentation.MAUI.Services;
using Presentation.MAUI.ViewModel;
using Presentation.MAUI.Views.Travel;



namespace Presentation.MAUI
{
    public static class PresentationDependencies
    {
        public static IServiceCollection AddPresentation(this IServiceCollection collection)
        {
            // - Transient : toujours une nouvelle instance
            // - Scoped    : une instance par scope (utile pour les pages)
            // - Singleton : une instance globale partagée


            collection.AddSingleton<INavigationService, NavigationService>();
            collection.AddScoped<TravelPageViewModel>();
            collection.AddScoped<NewTravelPageViewModel>();
            collection.AddScoped<TravelNotePageViewModel>();
            return collection;
        }
    }
}