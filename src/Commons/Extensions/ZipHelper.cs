using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Extensions
{
    public class ZipHelper
    {
        public static byte[] CreateZip(IEnumerable<byte[]?> files, string filePrefix = "file",string fileExtension="jpeg")
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
