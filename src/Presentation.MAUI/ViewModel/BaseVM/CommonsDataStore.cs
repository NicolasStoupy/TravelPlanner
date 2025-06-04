using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace Presentation.MAUI.ViewModel
{

    /// <summary>
    /// Data store for common application data such as activity types and currencies.
    /// </summary>
    public partial class CommonsDataStore : ObservableObject
    {
        private readonly IApplicationService _applicationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonsDataStore"/> class and loads common data.
        /// </summary>
        /// <param name="applicationService">The application service to retrieve data from.</param>
        public CommonsDataStore(IApplicationService applicationService)
        {
            _applicationService = applicationService;
            var ActivityTypesResult = _applicationService.ActivityService.GetActivitiesTypes();
            if (ActivityTypesResult.IsSuccess)
            {
                ActivityType = ActivityTypesResult.Value.ToObservableCollection();
            }
            var currenciesServiceResult = _applicationService.ExpenseService.GetCurrencies();
            if (currenciesServiceResult.IsSuccess)
            {
                Currencies = currenciesServiceResult.Value;
            }
        }
        /// <summary>
        /// Gets or sets the collection of activity types.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<TypeOfActivity>? _activityType;
        /// <summary>
        /// Gets or sets the list of currency codes or names.
        /// </summary>
        [ObservableProperty]
        private List<string>? _currencies;
    }
}