using System;
using System.IO;
using Ratbuddyssey.DTOs;
#if WPF_APP
#else
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
#endif

namespace Ratbuddyssey.Extensions;

public static class IOExtensions
{
#if WPF_APP
    public static FileData ToFile(
        this string path)
    {
        path = path ?? throw new ArgumentNullException(nameof(path));

        return new(
            Path.GetFileNameWithoutExtension(path),
            Path.GetExtension(path),
            File.ReadAllBytes(path));
    }
#else
    public static async Task<FileData> ToFileAsync(
        this StorageFile file,
        CancellationToken cancellationToken = default)
    {
        file = file ?? throw new ArgumentNullException(nameof(file));

        byte[] bytes;
        using (var stream = await file.OpenStreamForReadAsync().ConfigureAwait(false))
        using (var memoryStream = new MemoryStream())
        {
            await stream.CopyToAsync(
                memoryStream, 81920, cancellationToken).ConfigureAwait(false);

            bytes = memoryStream.ToArray();
        }

        var path = file.Name;

        return new(
            Path.GetFileNameWithoutExtension(path),
            Path.GetExtension(path),
            bytes);
    }
#endif
}
