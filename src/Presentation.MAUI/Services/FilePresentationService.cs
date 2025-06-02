using Commons;

using CommunityToolkit.Maui.Storage;
using Infrastructure.EntityModels;
using Presentation.MAUI.Interfaces;
using Presentation.MAUI.Models;

namespace Presentation.MAUI.Services
{
    /// <summary>
    /// Default implementation of <see cref="IFilePresentationService"/>,
    /// using MAUI’s FilePicker, FileSystem, and Launcher APIs.
    /// </summary>
    public class FilePresentationService : IFilePresentationService
    {
        private readonly IAlertService _alertService;

        public FilePresentationService(IAlertService alertService)
        {
            _alertService = alertService;
        }

        public async Task<byte[]?> LoadFileAsync(FilePickerFileType mediaType, string pickerTitle)
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = pickerTitle,
                FileTypes = mediaType
            });

            if (result == null)
                return null;

            try
            {
                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAsync(MessageType.Error,
                    $"Failed to load file: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]?> LoadTbinFile()
        {
            var tbinFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> { { DevicePlatform.WinUI, new[] { ".tbin" } } });
            var pickOptions = new PickOptions
            {
                PickerTitle = "Sélectionnez un fichier tbin",
                FileTypes = tbinFileType
            };
            var result = await FilePicker.PickAsync(pickOptions);

            if (result == null)
                return null;

            try
            {
                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                await _alertService.ShowAsync(MessageType.Error,
                    $"Failed to load file: {ex.Message}");
                return null;
            }

        }

        public async Task SaveFileAsync(byte[] file, string fileName, CancellationToken cancellationToken)
        {
            using var streamFile = new MemoryStream(file);
            var fileSaverResult = await FileSaver.Default.SaveAsync(fileName, streamFile, cancellationToken);
        }

        public async Task ShowFileAsync(byte[]? fileToShow)
        {
            if (fileToShow == null)
            {
                await _alertService.ShowAsync(MessageType.Warning,
                    "Unable to load the file.");
                return;
            }

            try
            {
                // Generate a unique filename in cache
                var fileGuid = Guid.NewGuid();
                var fileName = $"{fileGuid}.jpg";
                var cachePath = Path.Combine(FileSystem.CacheDirectory, fileName);

                // Write the file to cache
                File.WriteAllBytes(cachePath, fileToShow);

                // Open with native launcher
                var file = new ReadOnlyFile(cachePath);
                var request = new OpenFileRequest { File = file };
                await Launcher.OpenAsync(request);
            }
            catch (Exception ex)
            {
                await _alertService.ShowAsync(MessageType.Error,
                    $"Failed to display file: {ex.Message}");
            }
        }
    }
}
