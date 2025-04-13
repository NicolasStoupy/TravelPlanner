using System.ComponentModel.DataAnnotations;
using BussinessLogic.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Infrastructure.EntityModels;
using Presentation.MAUI.Models;
using Presentation.MAUI.Services;

namespace Presentation.MAUI.ViewModel
{
    /// <summary>
    /// ViewModel for creating a new travel entry.
    /// Handles form validation, image selection, and trip persistence via the application service layer.
    /// </summary>
    public partial class NewTravelPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        [Required(ErrorMessage = "Le nom du voyage est obligatoire.")]
        private string name = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "La description est obligatoire.")]
        private string description = string.Empty;

        [ObservableProperty]
        private DateTime startDate ;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="NewTravelPageViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The injected navigation service.</param>
        /// <param name="applicationService">The injected application/business service.</param>
        public NewTravelPageViewModel(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
            title = "Créer un nouveau voyage";
            currencyList = _applicationService.ExpenseService.GetCurrencies();
        }

        /// <summary>
        /// Opens the file picker to allow the user to select an image.
        /// If an image is selected, it is stored in the <see cref="TravelImage"/> property.
        /// </summary>
        [RelayCommand]
        private async Task LoadImage()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Sélectionner une image",
                FileTypes = FilePickerFileType.Images
            });

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();

                var imageLoaded = ImageSource.FromStream(() => stream);

                using var memoryStream = new MemoryStream();
                stream.Position = 0;
                await stream.CopyToAsync(memoryStream);
                TravelImage = memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Validates the form and saves the trip data if all fields are valid.
        /// Displays a success or error alert depending on the result.
        /// </summary>
        [RelayCommand]
        private async Task ValidateAndSave()
        {
            
            if (await base.ValidateAll())
            {
                //Success
                var result = await _applicationService.TravelService.SaveTrip(ToEntity(),TravelImage);
                if (result.IsSuccess)
                {
                    await BaseViewModel.DisplayAlert(MessageType.Success, "Voyage enregistré");
                    Reset();
                }
                else
                {
                    await BaseViewModel.DisplayAlert(MessageType.Error, result.ErrorMessage);
                }
            }
        }

        /// <summary>
        /// Converts the current ViewModel values into a <see cref="Trip"/> entity
        /// to be used for saving to the database.
        /// </summary>
        /// <returns>A new <see cref="Trip"/> entity populated with the ViewModel data.</returns>
        private Trip ToEntity()
        {
          

            return new Trip
            {
                Name = this.Name,
                Description = this.Description,
                StartDate = DateOnly.FromDateTime(StartDate),
                EndDate = DateOnly.FromDateTime(EndDate),
                Budget = this.Budget,
                NumberPeople = this.NumberPeople,            
                CurrencyCode = this.Currency
            };
        }

        /// <summary>
        /// Resets all form fields to their default state.
        /// </summary>
        public override void Reset()
        {
            Name = string.Empty;
            Description = string.Empty;
            StartDate = DateTime.Today;
            EndDate = DateTime.Today;
            Budget = 0;
            NumberPeople = 1;
            TravelImage = null;
            Currency = null;
        }

      
    }
}