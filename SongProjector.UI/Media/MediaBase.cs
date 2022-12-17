#nullable enable

using System;
using Windows.Storage;

namespace SongProjector.Media;

internal abstract class MediaBase : IMediaInfo, IMediaDisposable
{
    public MediaBase(StorageFile? file)
    {
        File = file;
        DisplayName = file?.DisplayName ?? "Unknown";
    }

    public StorageFile? File { get; }

    public string DisplayName { get; set; }

    public abstract int SlideCount { get; }


    public event Action? OnDispose;
    public void Dispose()
    {
        OnDispose?.Invoke();
    }
}
