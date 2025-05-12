using BussinessLogic.Interfaces;
using Commons;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.ViewModel
{
    public partial class MemoriesTravelVM : TravelVM
    {
        [ObservableProperty]
        private MemoryFile? _memoriesFiles ;

        [ObservableProperty]
        private ObservableCollection<byte[]>? _newFiles;
        public MemoriesTravelVM(INavigationService navigationService, IApplicationService applicationService) : base(navigationService, applicationService)
        {
            LoadData();
        }

        [RelayCommand]
        public void save()
        {

        }

        [RelayCommand]
        private async Task SendFiles(ObservableCollection<byte[]> files)
        {
            if (CurrentTravel?.Id != null)
            {
                await _applicationService.TravelService.AddMediaToTravel(files.ToList(), CurrentTravel.Id, TypeMedia.Images);
                 LoadData();
            }
            else
            {
               await  base.NoTravelSelected();
            }
                

        }

        private async void LoadData()
        {
            if (CurrentTravel?.Id != null)
            {
                MemoriesFiles = new MemoryFile();
               MemoriesFiles.Data= new ObservableCollection<byte[]>( _applicationService.MediaService.GetMediasFromTrip(CurrentTravel.Id, Commons.TypeMedia.Images));
            }
            else {

                await base.NoTravelSelected();
            }
            return;
        }
        public override void Reset()
        {
            LoadData();
            return;
        }
    }

    public class MemoryFile
    {

        public  ObservableCollection<byte[]> Data { get; set; } 
    }

}
