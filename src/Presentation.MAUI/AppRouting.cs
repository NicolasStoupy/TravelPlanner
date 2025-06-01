
using Presentation.MAUI.Views;
using Presentation.MAUI.Views.Activity;
using Presentation.MAUI.Views.Travel;

namespace Presentation.MAUI
{

    /// <summary>
    /// Class define the routing not implemented in the appshell
    /// </summary>
    public class AppRouting
    {
        /// <summary>
        /// Register all the routes 
        /// </summary>
        public static void RegisterRoutes()
        {
            Routing.RegisterRoute("ActivityNew", typeof(NewActivityPage));
            Routing.RegisterRoute("ActivityCost",typeof(NewCostActivityPage));
            Routing.RegisterRoute("ActivityFollower", typeof(ActivityFollowerPage));
            Routing.RegisterRoute("ImportExport", typeof(ImportExport));
            Routing.RegisterRoute("Notes", typeof(NoteTravelPage));
        }
    }
}