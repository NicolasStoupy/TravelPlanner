using BussinessLogic.Entities;
using Commons.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Models;
using Presentation.MAUI.Resources.Localization;
using System.Collections.ObjectModel;

namespace Presentation.MAUI.ViewModel
{ /// <summary>
  /// ViewModel responsible for handling import and export operations of <see cref="Travel"/> entities.
  /// </summary>
    public partial class ImportExportVM : BaseVM
    {
        /// <summary>
        /// Gets or sets the collection of travels displayed in the UI.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Travel> travels = new ObservableCollection<Travel>();

        [ObservableProperty]
        private string editorExplanation = Explication.ExplicationImport;


        /// <summary>
        /// Initializes a new instance of the <see cref="ImportExportVM"/> class with the specified services.
        /// </summary>
        /// <param name="viewModelServices">
        /// Provides access to application services such as travel management, dialogs, and alerts.
        /// </param>
        public ImportExportVM(IViewModelServices viewModelServices) : base(viewModelServices)
        {
        }

        /// <summary>
        /// Resets any stateful data in the view model.
        /// This method is called when the view model is first initialized or explicitly reset.
        /// </summary>
        public override void Reset()
        {
            return;
        }

        [RelayCommand]
        public async Task Export(Travel travel, CancellationToken cancellationToken = default)
        {
            var file = _services.Application.TravelService.ExportTravel(travel.Id);
            var fileName = _services.OutputFileNameProvider.GetFileName(Constants.ZIP_TRAVEL_EXPORT, travel.name);
            await _services.Alert.ShowAsync(file);
            if (file.IsSuccess)
                await _services.DialogFile.SaveFileAsync(file.Value, fileName, cancellationToken);
        }

        /// <summary>
        /// Prompts the user to select a TBIN file and imports its contents as a new <see cref="Travel"/> record.
        /// After import, the travel list is refreshed.
        /// </summary>
        [RelayCommand]
        public async Task Import()
        {
            var file = await _services.DialogFile.LoadTbinFile();
            if (file != null)
            {
                var resultService = await _services.Application.TravelService.ImportTravel(file);
                await _services.Alert.ShowAsync(resultService);
                if (resultService.IsSuccess)
                {
                    await _services.Alert.ShowAsync(Commons.MessageType.Success,DialogsStrings.IMPORT_SUCCESS,resultService.Value);
                }
            }
            await ResetAsync();
        }

        /// <summary>
        /// Asynchronously resets the view model by retrieving the latest list of travels
        /// from the travel service and populating the <see cref="Travels"/> collection.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public override async Task ResetAsync()
        {
            var travelsServiceResult = await _services.Application.TravelService.GetTravels();
            if (travelsServiceResult.IsSuccess)
            {
                Travels = travelsServiceResult.Value.ToObservableCollection();
            }
            return;
        }
    }
}