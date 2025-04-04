using Presentation.MAUI.ViewModel;


namespace Presentation.MAUI
{
    public static class PresentationDependencies
    {
        public static IServiceCollection AddPresentation(this IServiceCollection collection)
        {
            // - Transient : toujours une nouvelle instance
            // - Scoped    : une instance par scope (utile pour les pages)
            // - Singleton : une instance globale partagée

            collection.AddTransient<MainPage>();
            collection.AddTransient<TripViewModel>(); 
            collection.AddTransient<NewTripViewModel>();
            collection.AddTransient<TripMainPageViewModel>();
            return collection;
        }
    }
}