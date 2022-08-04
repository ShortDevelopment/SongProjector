using SongProjector.Presentation;
using SongProjector.Preview;

namespace SongProjector.Media
{
    public interface IMedia : IMediaInfo, IPreviewable, IPresentable, IMediaDisposable { }
}
