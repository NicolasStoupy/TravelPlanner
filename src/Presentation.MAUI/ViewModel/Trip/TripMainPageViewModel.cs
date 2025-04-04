using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Presentation.MAUI.Models;
using System.Collections.ObjectModel;


namespace Presentation.MAUI.ViewModel
{
    public partial class TripMainPageViewModel : BaseObservableObject
    {
        private readonly IApplicationService _applicationService;
        public ObservableCollection<TravelItem> TravelItems { get; } = new();

        [ObservableProperty]
        private  string? searchText;

        private readonly List<TravelItem> _travelItems;

        public TripMainPageViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
            var guid = new Guid("00000000-0000-0000-0000-000000000000");
         
            var file = _applicationService.MediaService.GetMedia(guid);
            title = "Mes Voyages";
            _travelItems = new List<TravelItem>()
            {    new TravelItem
    {
        name = "Parisss Escape",
        image = file, // image fictive
        description = "Un week-end romantique à Paris",
        travelDate = new DateOnly(2025, 5, 10),
        StartDate = new DateOnly(2025, 5, 8),
        EndDate = new DateOnly(2025, 5, 12)
    },

    new TravelItem
    {
        name = "Tokyo Tech Tour",
        image = file,
        description = "Découverte de la technologie japonaise",
        travelDate = new DateOnly(2025, 6, 20),
        StartDate = new DateOnly(2025, 6, 19),
        EndDate = new DateOnly(2025, 6, 25)
    },
    new TravelItem
    {
        name = "Sahara Adventure",
        image = file,
        description = "Exploration dans le désert du Sahara",
        travelDate = new DateOnly(2025, 9, 1),
        StartDate = new DateOnly(2025, 8, 31),
        EndDate = new DateOnly(2025, 9, 5)
    },
    new TravelItem
    {
        name = "New York City Lights",
        image = file,
        description = "La ville qui ne dort jamais",
        travelDate = new DateOnly(2025, 12, 24),
        StartDate = new DateOnly(2025, 12, 22),
        EndDate = new DateOnly(2025, 12, 28)
    },
    new TravelItem
    {
        name = "Iceland Northern Lights",
        image = file,
        description = "Voyage pour voir les aurores boréales",
        travelDate = new DateOnly(2026, 1, 15),
        StartDate = new DateOnly(2026, 1, 14),
        EndDate = new DateOnly(2026, 1, 20)
    }, new TravelItem
    {
        name = "Paris Escape",
        image = file, // image fictive
        description = "Un week-end romantique à Paris",
        travelDate = new DateOnly(2025, 5, 10),
        StartDate = new DateOnly(2025, 5, 8),
        EndDate = new DateOnly(2025, 5, 12)
    },

    new TravelItem
    {
        name = "Tokyo Tech Tour",
        image = file,
        description = "Découverte de la technologie japonaise",
        travelDate = new DateOnly(2025, 6, 20),
        StartDate = new DateOnly(2025, 6, 19),
        EndDate = new DateOnly(2025, 6, 25)
    },
    new TravelItem
    {
        name = "Sahara Adventure",
        image = file,
        description = "Exploration dans le désert du Sahara",
        travelDate = new DateOnly(2025, 9, 1),
        StartDate = new DateOnly(2025, 8, 31),
        EndDate = new DateOnly(2025, 9, 5)
    },
    new TravelItem
    {
        name = "New York City Lights",
        image = file,
        description = "La ville qui ne dort jamais",
        travelDate = new DateOnly(2025, 12, 24),
        StartDate = new DateOnly(2025, 12, 22),
        EndDate = new DateOnly(2025, 12, 28)
    },
    new TravelItem
    {
        name = "Iceland Northern Lights",
        image = file,
        description = "Voyage pour voir les aurores boréales",
        travelDate = new DateOnly(2026, 1, 15),
        StartDate = new DateOnly(2026, 1, 14),
        EndDate = new DateOnly(2026, 1, 20)
    } };
            FilterItems();
        }

        partial void OnSearchTextChanged(string value)
        {
            FilterItems();
        }
        private async void FilterItems()
        {
            IsBusy = true;
            TravelItems.Clear();
            await Task.Delay(10);
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? _travelItems
                : _travelItems.Where(item =>
                    item.name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    item.description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var item in filtered)
                TravelItems.Add(item);

            IsBusy = false;
        }

       
    }






}


