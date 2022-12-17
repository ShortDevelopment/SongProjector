using Windows.Storage;

namespace SongProjector.Media;

public interface IMediaInfo
{
    StorageFile File { get; }
    string DisplayName { get; }
    int SlideCount { get; }
}
