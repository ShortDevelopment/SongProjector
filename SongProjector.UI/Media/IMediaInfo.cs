﻿using Windows.Storage;

namespace SongProjector.Media
{
    public interface IMediaInfo
    {
        StorageFile File { get; }
        int SlideCount { get; }
    }
}
