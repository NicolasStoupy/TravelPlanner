using Presentation.MAUI.ViewModel;


namespace Presentation.MAUI
{
    public static class PresentationDependencies
    {
        public static IServiceCollection AddPresentation(this IServiceCollection collection)
        {
            _ = collection.AddTransient<TripViewModel>();
            _ = collection.AddTransient<NewTripViewModel>();
            return collection;
        }
    }
}