using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Models;
using Presentation.MAUI.Services;
using System.Collections.ObjectModel;

namespace Presentation.MAUI.ViewModel;
public partial class TravelPageViewModel : BaseObservableObject
{
    private readonly IApplicationService _applicationService;
 
    private List<TravelItem> _allTravelItems = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<TravelItem> _travelItems = new();

    public TravelPageViewModel(IApplicationService applicationService, INavigationService navigationService) : base(navigationService)
    {
        Title = "Voyages";
        _applicationService = applicationService;       

        _allTravelItems = _applicationService.TravelService.GetTravelItems();
        FilterItems(); // premier affichage
    }

    private IEnumerable<TravelItem> GetFilteredItems()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
            return _allTravelItems;

        return _allTravelItems
            .Where(item =>
                (!string.IsNullOrEmpty(item.name) && item.name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(item.description) && item.description.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            );
    }

    partial void OnSearchTextChanged(string value)
    {
        isBusy = true;
        FilterItems();
        isBusy = false;
    }

    [RelayCommand]
    private void FilterItems()
    {
        IsBusy = true;
        TravelItems = new ObservableCollection<TravelItem>(GetFilteredItems());
        IsBusy = false;
    }

    [RelayCommand]
    private async Task TravelDetails(TravelItem travelItem)
    {
        if (travelItem is null)
            return;

        await _navigationService.NavigateToTravelDetailsPageAsync(travelItem);
    }

    [RelayCommand]
    private async Task NewTravel()
    {
        IsBusy = true;

        await _navigationService.NavigateToNewTravelPageAsync();
    }
}
