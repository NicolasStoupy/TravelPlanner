using System.IO.Compression;

namespace Commons.Extensions
{
    /// <summary>
    /// Provides helper methods for creating ZIP archives from byte arrays.
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// Creates a ZIP archive containing the provided files, each named with a sequential prefix.
        /// </summary> <param name="files">An enumerable of byte arrays representing file contents.
        /// Null or empty byte arrays are skipped.</param> <param name="filePrefix">The prefix used for
        /// naming each file entry in the ZIP. Defaults to "file".</param> <param
        /// name="fileExtension">The extension assigned to each file in the ZIP. Defaults to
        /// "jpeg".</param> <returns>A byte array containing the complete ZIP archive.</returns>
        public static byte[] CreateZip(IEnumerable<byte[]?> files, string filePrefix = "file", string fileExtension = "jpeg")
        {
            using var zipStream = new MemoryStream();

            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                int i = 1;
                foreach (var file in files)
                {
                    if (file is { Length: > 0 })
                    {
                        var entry = archive.CreateEntry($"{filePrefix}_{i:D3}.{fileExtension}", CompressionLevel.Optimal);
                        using var entryStream = entry.Open();
                        entryStream.Write(file, 0, file.Length);
                        i++;
                    }
                }
            }

            zipStream.Seek(0, SeekOrigin.Begin);
            return zipStream.ToArray(); ;
        }
    }
}