
using Presentation.MAUI.Views.Activity;

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
        }
    }
}