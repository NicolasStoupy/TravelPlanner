using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using Infrastructure.EntityModels;

namespace Presentation.MAUI.ViewModel.Models
{
    public partial class TravelModel : ObservableValidator
    {
        [ObservableProperty]
        [Required(ErrorMessage = "Le nom du voyage est obligatoire.")]
        private string name = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "La description est obligatoire.")]
        private string description = string.Empty;

        [ObservableProperty]
        private DateTime startDate;

        [ObservableProperty]
        private DateTime endDate;

        [ObservableProperty]
        [Range(0, 999999, ErrorMessage = "Le budget doit être positif.")]
        private decimal budget;

        [ObservableProperty]
        [Range(1, 999, ErrorMessage = "Au moins 1 participant est requis.")]
        private int numberPeople;

        [ObservableProperty]
        private byte[]? travelImage;

        [ObservableProperty]
        [Required(ErrorMessage = "La devise est obligatoire")]
        private string? currency;

        [ObservableProperty]
        private List<string> currencyList = new List<string>();

        
    }
}