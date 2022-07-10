using SongProjector.Presentation;
using SongProjector.Preview;
using System;

namespace SongProjector.Media
{
    public interface IMedia : IMediaInfo, IPreviewable, IPresentable, IDisposable { }
}
