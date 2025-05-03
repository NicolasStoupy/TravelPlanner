
using Presentation.MAUI.Views.Activity;

namespace Presentation.MAUI
{
    public class AppRouting
    {
        public static void RegisterRoutes()
        {
            Routing.RegisterRoute("ActivityNew", typeof(NewActivityPage));
        }
    }
}