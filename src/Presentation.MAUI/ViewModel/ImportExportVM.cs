using BussinessLogic.Entities;
using Commons.Extensions;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Storage;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.MAUI.ViewModel
{
    public partial class ImportExportVM : BaseVM
    {
        [ObservableProperty]
        private ObservableCollection<Travel> travels = new ObservableCollection<Travel>();


        public ImportExportVM(IViewModelServices viewModelServices) : base(viewModelServices)
        {

        }

        public override void Reset()
        {
            return;
        }
        [RelayCommand]
        public async Task Export(Travel travel, CancellationToken cancellationToken = default)
        {
            var file = _services.Application.TravelService.ExportTravel(travel.Id);
            var fileName = _services.OutputFileNameProvider.GetFileName(Constants.ZIP_TRAVEL_EXPORT, travel.name);
            if (file.IsSuccess)
            {
                using var zipFile = new MemoryStream(file.Value);
                var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, zipFile, cancellationToken);

            }

        }
        [RelayCommand]
        public async Task Import()
        {
            var file = await _services.DialogFile.LoadTbinFile();
            if (file != null)
            {
                var resultService = _services.Application.TravelService.ImportTravel(file);
            }

        }
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