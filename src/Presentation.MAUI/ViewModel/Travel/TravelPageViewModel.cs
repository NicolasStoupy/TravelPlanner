using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Models;
using Presentation.MAUI.Services;
using System.Collections.ObjectModel;

namespace Presentation.MAUI.ViewModel;
public partial class TravelPageViewModel : BaseViewModel
{
    
 
    private List<TravelItem> _allTravelItems = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]   
    private ObservableCollection<TravelItem> _travelItems = new();

    public TravelPageViewModel(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
    {
        Title = "Voyages";
        Reset();
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
        IsBusy = false;
    }

    [RelayCommand]
    private async Task DeleteTravel(int tripId)
    {
        IsBusy= true;
        try
        {
            await _applicationService.TravelService.DeleteTrip(tripId);
            await DisplayAlert(MessageType.Success, "Trip Supprimé avec succès");

            Reset();
        }
        catch (Exception ex )
        {

           await DisplayAlert(MessageType.Error, ex.Message);
        }
     
        IsBusy = false;
    }

    public override void Reset()
    {
        IsBusy = true;
        _allTravelItems.Clear();
        TravelItems.Clear();
        _allTravelItems = _applicationService.TravelService.GetTravelItems();
        TravelItems = new ObservableCollection<TravelItem>(_allTravelItems);
        IsBusy = false;
    }
}
