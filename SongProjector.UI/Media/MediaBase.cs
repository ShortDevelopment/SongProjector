using System;
using Windows.Storage;

namespace SongProjector.Media
{
    internal abstract class MediaBase : IMediaInfo, IMediaDisposable
    {
        public MediaBase(StorageFile file)
        {
            File = file;
        }

        public StorageFile File { get; }

        public abstract int SlideCount { get; }


        public event Action OnDispose;
        public void Dispose()
        {
            OnDispose?.Invoke();
        }
    }
}
