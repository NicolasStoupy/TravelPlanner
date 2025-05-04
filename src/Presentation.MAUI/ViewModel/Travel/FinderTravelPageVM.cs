using BussinessLogic.Interfaces;
using BussinessLogic.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Models;
using Presentation.MAUI.Services;
using System.Collections.ObjectModel;

namespace Presentation.MAUI.ViewModel;

public partial class FinderTravelPageVM : TravelVM
{
    private List<Travel> _allTravelItems = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Travel> _travelItems = [];

    public FinderTravelPageVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
    {
        Title = "Voyages";
       
        FilterItems();
    }

    /// <summary>
    /// Returns a filtered list of travel items based on the search text.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{TravelItem}"/> containing all travel items
    /// whose name or description contains the specified search text,
    /// ignoring case. If the search text is null or whitespace, all items are returned.
    /// </returns>
    private IEnumerable<Travel> GetFilteredItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return _allTravelItems;

        return _allTravelItems
            .Where(item =>
                (!string.IsNullOrEmpty(item.name) && item.name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(item.description) && item.description.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            );
    }

    /// <summary>
    /// Triggered when the search text value changes.
    /// Sets the busy state, filters the travel items accordingly,
    /// and then resets the busy state.
    /// </summary>
    /// <param name="value">The new search text entered by the user.</param>
    partial void OnSearchTextChanged(string value)
    {
        isBusy = true;
        FilterItems();
        isBusy = false;
    }

    /// <summary>
    /// Filters the list of travel items based on the current search text
    /// and updates the <see cref="TravelItems"/> collection.
    /// Sets the busy state while filtering is in progress.
    /// </summary>
    [RelayCommand]
    private void FilterItems()
    {
        IsBusy = true;
        TravelItems = [.. GetFilteredItems()];
        IsBusy = false;
    }

    /// <summary>
    /// Navigates to the travel details page for the specified travel item.
    /// </summary>
    /// <param name="travelItem">The <see cref="Travel"/> to display details for. If null, the method exits.</param>
    [RelayCommand]
    private async Task TravelDetails(Travel travelItem)
    {
        if (travelItem is null)
            return;

        await _navigationService.NavigateToNewTravel(travelItem.Id.ToString());
    }

    /// <summary>
    /// Navigates to the page for creating a new travel entry.
    /// Sets the busy state during the navigation process.
    /// </summary>
    [RelayCommand]
    private async Task NewTravel()
    {
        IsBusy = true;

        await _navigationService.NavigateToNewTravelPageAsync();
        IsBusy = false;
    }

    /// <summary>
    /// Deletes the travel entry with the specified ID, displays a confirmation alert,
    /// and resets the state. Sets the busy state during the operation.
    /// </summary>
    /// <param name="tripId">The ID of the trip to delete.</param>
    [RelayCommand]
    private async Task DeleteTravel(int tripId)
    {
        IsBusy = true;

        await DisplayAlert(await _applicationService.TravelService.DeleteTravel(tripId));
        Reset();
        IsBusy = false;
    }

    /// <summary>
    /// Resets the travel item lists by clearing and reloading them
    /// from the travel service. Sets the busy state during the operation.
    /// </summary>
    public override void Reset()
    {
        IsBusy = true;
        _allTravelItems.Clear();
        TravelItems.Clear();
        _allTravelItems = _applicationService.TravelService.GetTravels();
        TravelItems = [.. _allTravelItems];
        IsBusy = false;
    }
}