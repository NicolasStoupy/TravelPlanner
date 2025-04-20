
using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Extensions;
using Commons.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Presentation.MAUI.Models;
using Presentation.MAUI.Services;
using Presentation.MAUI.Validators;

namespace Presentation.MAUI.ViewModel
{
    /// <summary>
    /// ViewModel for creating a new travel entry.
    /// Handles form validation, image selection, and trip persistence via the application service layer.
    /// </summary>

    [QueryProperty(nameof(TravelID), "travelID")]
    public partial class NewTravelPageViewModel : BaseViewModel
    {
        [ObservableProperty]
        private string _travelID;
        partial void OnTravelIDChanged(string value) => NavigationDetails(value);
        [ObservableProperty]
        private Travel _travel = new();
        partial void OnTravelChanged(Travel value) => Travel = value;

        [ObservableProperty]
        private List<string> _currencyList = new();

        [ObservableProperty]
        private byte[]? _imageSelected;
        partial void OnImageSelectedChanged(byte[]? value) => Travel.image = value;
        [ObservableProperty]
        private string? _currencySelected;
        partial void OnCurrencySelectedChanged(string? value) => Travel.currencie = value;
        protected override IValidator? GetValidator() => new NewTravelPageViewModelValidator();

        [ObservableProperty]
        private Mode _currentMode = Mode.New;
        partial void OnCurrentModeChanged(Mode value) => CurrentModeFriendly = value.ToDisplayName();

        [ObservableProperty]
        private string _currentModeFriendly;

        #region ChangeEnventBehavior     



        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="NewTravelPageViewModel"/> class.
        /// </summary>
        /// <param name="navigationService">The injected navigation service.</param>
        /// <param name="applicationService">The injected application/business service.</param>
        public NewTravelPageViewModel(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
            title = "Créer un nouveau voyage";
            CurrencyList = _applicationService.ExpenseService.GetCurrencies();
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
                ImageSelected = memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Validates the form and saves the trip data if all fields are valid.
        /// Displays a success or error alert depending on the result.
        /// </summary>
        [RelayCommand]
        private async Task ValidateAndSave()
        {
            if (!await ValidateAll())
                return;

            Result result;

            switch (CurrentMode)
            {
                case Mode.New:
                    result = await _applicationService.TravelService.SaveTravel(Travel);
                    await HandleResultAndReset(result, true);
                    break;

                case Mode.Edit:
                 
                    result = await _applicationService.TravelService.UpdateTravel(Travel);
                    await HandleResultAndReset(result, false);
                    break;

                default:
                    result = Result.Failure("Mode de traitement inconnu.");
                    await HandleResultAndReset(result, false);
                    break;
            }

        }
        private void NavigationDetails(string value)
        {
            if (value == null)
            {
                Reset();
                CurrentMode = Mode.New;
            }
            else
            {
                int travelId = int.Parse(value);
                CurrentMode = Mode.Edit;
                Travel = _applicationService.TravelService.GetTravel(travelId);
                ImageSelected = Travel.image;
                CurrencySelected = Travel.currencie;
                CurrentTravel= Travel;
            }
        }



        /// <summary>
        /// Resets all form fields to their default state.
        /// </summary>
        public override void Reset()
        {
            Travel = new Travel();
            ImageSelected = null;
            CurrencySelected = null;
        }



    }
}