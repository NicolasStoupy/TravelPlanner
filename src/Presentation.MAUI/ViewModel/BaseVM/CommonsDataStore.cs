using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Presentation.MAUI.ViewModel
{
    public partial class CommonsDataStore : ObservableObject
    {
        private readonly IApplicationService _applicationService;

        public CommonsDataStore(IApplicationService applicationService)
        {
            _applicationService = applicationService;
            var ActivityTypesResult = _applicationService.ActivityService.GetActivitiesTypes();
            if (ActivityTypesResult.Status.IsSuccess)
            {
                ActivityType = ActivityTypesResult.Value.ToObservableCollection();
            }
            Currencies = _applicationService.ExpenseService.GetCurrencies();
        }

        [ObservableProperty]
        private ObservableCollection<TypeOfActivity>? _activityType;

        [ObservableProperty]
        private List<string>? _currencies;
    }
}