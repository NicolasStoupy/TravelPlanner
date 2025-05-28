using BussinessLogic.Entities;
using Commons;
using Commons.Extensions;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Resources.Localization;
using Presentation.MAUI.Views;
using System.Collections.ObjectModel;

namespace Presentation.MAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for managing travel memories (images) associated with a trip.
    /// Provides functionality to import, delete, and export selected media files.
    /// </summary>   
    public partial class MemoriesTravelVM : TravelVM
    {
        [ObservableProperty]
        private bool _extraAction;

        /// <summary>
        /// Collection of memory files (images) linked to the current travel.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MemoryFile>? _memoriesFiles;

        /// <summary>
        /// Determines whether extra UI actions (e.g. delete, export) are enabled.
        /// </summary>
        /// <summary>
        /// Temporary collection of newly uploaded files (as byte arrays).
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<MemoryFile>? _newFiles;

        /// <summary>
        /// Constructor that initializes the ViewModel with dependencies and loads data.
        /// </summary>
        public MemoriesTravelVM(IViewModelServices viewModelServices) : base(viewModelServices)
        {
            LoadData();
        }

        /// <summary>
        /// Command that deletes the currently selected memory files after confirmation.
        /// </summary>
        [RelayCommand]
        public async Task DeleteSelected()
        {
            if (MemoriesFiles != null && CurrentTravel != null)
            {
                var selectedMemories = GetSelectedMemories();

                if (selectedMemories != null)
                {
                    bool confirm = await _services.Alert.ConfirmAsync(
                        DialogsStrings.DeleteMemories_Title,
                        DialogsStrings.DeleteMemories_Confirmation,
                        DialogsStrings.CommonsYes, DialogsStrings.CommonsYes, selectedMemories.Count());
                    if (!confirm)
                        return;
                }
                var result = await _services.Application.TravelService.RemoveMemories(selectedMemories, CurrentTravel.Id);
                await _services.Alert.ShowAsync(result);

                LoadData();
            }
        }

        /// <summary>
        /// Exports selected memory files to a ZIP archive and prompts the user to save it.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        [RelayCommand]
        public async Task ExportSelectedAsZip(CancellationToken cancellationToken)
        {
            var selectedMemories = GetSelectedMemories();
            var fileName = _services.OutputFileNameProvider.GetFileName(Constants.ZIP_MEMORIES_CONFIG, CurrentTravel?.name);
            if (selectedMemories != null && selectedMemories.Any())
            {
                var zipBytes = ZipHelper.CreateZip(selectedMemories.Select(m => m.Files));
                using var stream = new MemoryStream(zipBytes);
                var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, stream, cancellationToken);
            }
        }

        /// <summary>
        /// Indicates whether at least one memory is selected.
        /// Used to toggle UI actions.
        /// </summary>
        /// <returns>True if any memory is selected; otherwise, false.</returns>
        public bool ExtraActionIsEnabled()
        {
            if (MemoriesFiles != null)
            {
                return MemoriesFiles.Where(m => m.Checked).Count() > 0;
            }
            return false;
        }

        /// <summary>
        /// Reloads the current data state from the source.
        /// </summary>
        public override void Reset()
        {
            LoadData();
            return;
        }

        // <summary>
        /// Returns the list of currently selected memory files. </summary> <returns>A filtered list
        /// of memory files marked as checked.</returns>
        private IEnumerable<MemoryFile>? GetSelectedMemories()
        {
            return MemoriesFiles?.Where(m => m.Checked == true) ?? new List<MemoryFile>();
        }

        /// <summary>
        /// Loads the current travel's memory files into the ViewModel.
        /// </summary>
        private async void LoadData()
        {
            ExtraAction = false;
            if (CurrentTravel?.Id != null)
            {
                MemoriesFiles = new ObservableCollection<MemoryFile>(_services.Application.TravelService.GetMemories(CurrentTravel.Id, Commons.TypeMedia.Images));
            }
            else
            {
                await base.NoTravelSelected();
            }
            return;
        }

        /// <summary>
        /// Triggered when the MemoriesFiles collection is updated.
        /// Enables or disables extra actions based on selection.
        /// </summary>
        partial void OnMemoriesFilesChanged(ObservableCollection<MemoryFile>? value)
        {
            ExtraAction = false;
            if (value != null)
            {
                ExtraAction = ExtraActionIsEnabled();
            }
        }

        /// <summary>
        /// Uploads the provided files to the current travel's media collection.
        /// </summary>
        /// <param name="files">The files to upload.</param>

        [RelayCommand]
        private async Task SendFiles(ObservableCollection<byte[]> files)
        {
            if (CurrentTravel?.Id != null)
            {
                var result = await _services.Application
                                            .TravelService
                                            .AddMediaToTravel(files.ToList(), CurrentTravel.Id, TypeMedia.Images);
                if ((result.Status.IsSuccess))
                {
                    Reset();
                }
                else
                {
                    await _services.Alert.ShowAsync(result.Status);
                }
            }
            else
            {
                await base.NoTravelSelected();
            }
        }

        [RelayCommand]
        private void SelectAll()
        {
            foreach (var item in MemoriesFiles)
            {
                item.Checked = true;
            }
            MemoriesFiles = new ObservableCollection<MemoryFile>(MemoriesFiles);

        }

        [RelayCommand]
        private Task OnEditorUnfocused(MemoryFile memory)
        {
            _services.Application.TravelService.UpdateMemory(memory);

            return Task.CompletedTask;
        }
        [RelayCommand]
        public async Task ShowImage(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return;
            await Shell.Current.Navigation.PushAsync(new FullScreenImagePage(imageBytes));
        }

    }
}