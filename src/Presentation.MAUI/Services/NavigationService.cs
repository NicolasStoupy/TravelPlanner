using BussinessLogic.Entities;
using Commons.Extensions;
using Presentation.MAUI.Interfaces;

namespace Presentation.MAUI.Services
{
    public class NavigationService : INavigationService
    {
        private async Task Navigate(string query)
        {
            
                await Shell.Current.GoToAsync(query);
            

        }
        public async Task NavigateToNewTravelPageAsync()
        {
            await Navigate($"//TravelInformations");
        }

        public async Task NavigateToNewTravel(string travelID)
        {

            await Navigate($"//TravelInformations?travelID={travelID}");
        }

        public async Task NavigateToTravelFinder()
        {
            await Navigate("//TravelFinder");
        }

        public async Task NavigateToNewActivity()
        {
            await Navigate("ActivityNew");
        }

        public async Task GoBack()
        {
            await Navigate("..");
        }

        public async Task NavigateToEditActivity(TravelActivity travelActivity)
        {

            await Navigate($"ActivityNew?ActivityID={travelActivity.ActivityID}");
        }

        public async Task NavigateToActivityCost(int activityID)
        {
            await Navigate($"ActivityCost?ActivityID={activityID}");
        }

        public async Task NavigateToActivityFollower(int activityID)
        {
            await Navigate($"ActivityFollower?ActivityID={activityID}");
        }

        public async Task NavigateToTravelActivities()
        {
            await Navigate($"TravelActivities");
        }

        public async Task GoHome() => await NavigateToTravelFinder();

        public async Task NavigationToNoteForActivity(int activityID)
        {
            await Navigate($"Notes?ActivityID={activityID}");
        }

        public async Task NavigationToNoteForTravel(int travelID)
        {
            await Navigate($"//Notes?TravelID={travelID}");
        }
    }
}
