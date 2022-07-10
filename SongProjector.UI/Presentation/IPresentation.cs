using SongProjector.Media;

namespace SongProjector.Presentation
{
    internal interface IPresentation
    {
        void Present(IMedia media, int slideIndex);
        void Blank();
    }
}
