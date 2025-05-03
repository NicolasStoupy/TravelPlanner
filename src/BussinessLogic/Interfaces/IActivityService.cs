using BussinessLogic.Entities;
using Commons.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogic.Interfaces
{
    public interface IActivityService
    {
       List<Infrastructure.EntityModels.ActivityType> GetActivitiesTypes();
        Task<Result> SaveNewActivity(int travelID, Infrastructure.EntityModels.Activity newActivity);

        Task<Result> UpdateActivity(Infrastructure.EntityModels.Activity travelActivity);
    }
}
