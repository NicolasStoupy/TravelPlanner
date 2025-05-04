using BussinessLogic.Entities;
using Commons.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Interfaces
{
    public interface IActivityService
    {
        Task<Result> DeleteActivity(TravelActivity activity);
        List<TravelActivity> GetActivities(int travelID);
        List<TypeOfActivity> GetActivitiesTypes();
        Task<Result> SaveNewActivity(TravelActivity newActivity);

        Task<Result> UpdateActivity(TravelActivity travelActivity);
    }
}
