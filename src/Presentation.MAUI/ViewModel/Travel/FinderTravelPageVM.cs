using BussinessLogic.Entities;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Storage;
using Presentation.MAUI.Resources.Localization;
using Presentation.MAUI.Interfaces;

namespace Presentation.MAUI.ViewModel;

public partial class FinderTravelPageVM : TravelVM
{

    List<Travel> _allTravelItems = [];

    [ObservableProperty] string _searchText = string.Empty;

    [ObservableProperty] ObservableCollection<Travel> _travelItems = [];


    public FinderTravelPageVM(IViewModelServices viewModelServices) : base(viewModelServices)
    {
        Title = PageTitle.FinderTravelPage;
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

        await _services.Navigation.NavigateToNewTravel(travelItem.Id.ToString());
    }

    /// <summary>
    /// Navigates to the page for creating a new travel entry.  
    /// </summary>
    [RelayCommand]
    private async Task NewTravel() => await _services.Navigation.NavigateToNewTravelPageAsync();


    /// <summary>
    /// Deletes the travel entry with the specified ID, displays a confirmation alert,
    /// and resets the state. Sets the busy state during the operation.
    /// </summary>
    /// <param name="tripId">The ID of the trip to delete.</param>
    [RelayCommand]
    private async Task DeleteTravel(int tripId)
    {
        IsBusy = true;
        bool confirm = await _services.Alert.ConfirmAsync(
            DialogsStrings.DeleteTravel_Title,
            DialogsStrings.DeleteTravel_Confirmation,
            DialogsStrings.DeleteTravel_Yes,
            DialogsStrings.DeleteTravel_No);

        if (!confirm)
        {
            IsBusy = false;
            return;
        }

        var result = await _services.Application.TravelService.DeleteTravel(tripId);
        await _services.Alert.ShowAsync(result.Status, true);
        Reset();
        IsBusy = false;
    }

    /// <summary>
    /// Resets the travel item lists by clearing and reloading them
    /// from the travel service. Sets the busy state during the operation.
    /// </summary>
    public override async void Reset()
    {
        IsBusy = true;
        _allTravelItems.Clear();
        TravelItems.Clear();
        var result = await _services.Application.TravelService.GetTravels();
        if (result.Status.IsSuccess)
        {
            _allTravelItems = result.Value;
            TravelItems = [.. _allTravelItems];
            IsBusy = false;
        }
        else
        {

            await _services.Alert.ShowAsync(result.Status);
        }
        return;
    }

    [RelayCommand]
    public async Task ExtractTravelToPdf(Travel travel, CancellationToken cancellationToken)
    {
        var PDFFile = _services.Application.MediaService.GeneratePdfSummary(travel);
        var fileName = _services.OutputFileNameProvider.GetFileName(Constants.PDF_TRAVEL_CONFIG, travel.name);

        using var pdfFileStream = new MemoryStream(PDFFile);
        var fileSaverResult = await FileSaver.Default
            .SaveAsync(fileName, pdfFileStream, cancellationToken);

    }

    [RelayCommand]
    public async Task CloneTravel(Travel travel)
    {
        var result = await _services.Application.TravelService.CloneTravel(travel);
        if (!result.IsSuccess)
            await _services.Alert.ShowAsync(result.Status);
        Reset();
    }
}