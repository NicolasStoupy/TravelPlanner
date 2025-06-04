using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BussinessLogic.Entities;

namespace Presentation.MAUI.Interfaces
{
    /// <summary>
    /// Defines navigation operations between pages and views within the application.
    /// </summary>
    public interface INavigationService
    {

        Task NavigateToNewTravelPageAsync(bool newTravel);
        Task NavigateToNewTravel(string travelID);
        Task NavigateToTravelFinder();
        Task NavigateToNewActivity();
        Task NavigateToEditActivity(TravelActivity travelActivity);
        Task GoBack();
        Task NavigateToActivityCost(int activityID);
        Task NavigateToActivityFollower(int activityID);

        Task NavigateToTravelActivities();
        Task GoHome();

        Task NavigationToNoteForActivity(int activityID);

        Task NavigationToNoteForTravel(int travelID);
    }
}
