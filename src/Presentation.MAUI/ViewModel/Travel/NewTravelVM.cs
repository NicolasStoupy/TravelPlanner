using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Commons.Extensions;
using Commons.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentValidation;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Resources.Localization;
using Presentation.MAUI.Validators;

namespace Presentation.MAUI.ViewModel
{
    /// <summary>
    /// ViewModel for creating a new travel entry.
    /// Handles form validation, image selection, and trip persistence via the application service layer.
    /// </summary>

    [QueryProperty(nameof(TravelID), "travelID")]
    public partial class NewTravelVM : TravelVM
    {
        [ObservableProperty] private string _travelID;

        partial  void OnTravelIDChanged(string value) => NavigationDetails(value);

        [ObservableProperty] private Travel _travel = new();

        partial void OnTravelChanged(Travel value) => Travel = value;

        [ObservableProperty] private List<string> _currencyList = new();

        [ObservableProperty] private byte[]? _imageSelected;

        partial void OnImageSelectedChanged(byte[]? value) => Travel.image = value;

        [ObservableProperty] private string? _currencySelected;

        partial void OnCurrencySelectedChanged(string? value) => Travel.currencie = value;

   

        [ObservableProperty]
        private Mode _currentMode = Mode.New;

        partial void OnCurrentModeChanged(Mode value) => CurrentModeFriendly = value.ToDisplayName();

        [ObservableProperty]
        private string _currentModeFriendly;

       
        public NewTravelVM(IViewModelServices viewModelServices) : base(viewModelServices)
        {
            title = PageTitle.NewTravelPage;

            CurrencyList = _dataStore.Currencies.ToList();
        }

        /// <summary>
        /// Opens the file picker to allow the user to select an image.
        /// If an image is selected, it is stored in the <see cref="TravelImage"/> property.
        /// </summary>
        [RelayCommand]
        private async Task LoadImage()
        {
            ImageSelected = await _services.DialogFile.LoadFileAsync(FilePickerFileType.Images, "Sélectionner une image");
        }

        /// <summary>
        /// Validates the form and saves the trip data if all fields are valid.
        /// Displays a success or error alert depending on the result.
        /// </summary>
        [RelayCommand]
        private async Task ValidateAndSave()
        {
            if(!await _services.Validation.ValidateAndNotifyAsync(this))
            {
                return;
            }
            Result result;

            switch (CurrentMode)
            {
                case Mode.New:
                    result = await _services.Application.TravelService.SaveTravel(Travel);
                    await _services.Alert.HandleResultAndResetAsync(result,this, true);
                    break;

                case Mode.Edit:

                    result = await _services.Application.TravelService.UpdateTravel(Travel);
                    await _services.Alert.HandleResultAndResetAsync(result,this, false);
                    break;

                default:
                    result = Result.Failure(ExceptionMessage.UknowMode);
                    await _services.Alert.HandleResultAndResetAsync(result,this, false);
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

                Travel = _services.Application.TravelService.GetTravel(travelId);
                ImageSelected = Travel.image;
                CurrencySelected = Travel.currencie;
                CurrentTravel = Travel;
                if (CurrentMode == Mode.Edit && CurrentTravel != null)
                {
                    Title = CurrentTravel?.description??string.Empty;
                }

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
            return;
        }
    }
}