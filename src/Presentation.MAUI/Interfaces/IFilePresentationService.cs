using Commons.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.MAUI.Interfaces
{
    /// <summary>
    /// Handles loading files via the native file picker and displaying them
    /// using the native file launcher, with built-in error reporting.
    /// </summary>
    public interface IFilePresentationService
    {
        /// <summary>
        /// Prompts the user to pick a file of the specified type, then returns its bytes.
        /// </summary>
        /// <param name="mediaType">The set of allowed file types for the picker.</param>
        /// <param name="pickerTitle">The title to display on the system file picker.</param>
        /// <returns>
        /// A <see cref="Task{ByteArray}"/> that yields the selected file’s bytes,
        /// or <c>null</c> if the user cancelled or an error occurred.
        /// </returns>
        Task<byte[]?> LoadFileAsync(FilePickerFileType mediaType, string pickerTitle);
        Task <byte[]?> LoadTbinFile();
        Task SaveFileAsync(byte[] file, string fileName, CancellationToken cancellationToken);

        /// <summary>
        /// Writes the provided byte array to a temporary cache file and opens it
        /// with the native file launcher. Shows a warning if the byte array is null.
        /// </summary>
        /// <param name="fileToShow">The file data to display.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ShowFileAsync(byte[]? fileToShow);
    }
}
