using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace SongProjector.Presentation
{
    public interface IPresentable
    {
        Task<FrameworkElement> GeneratePresentationAsync(int slideId, Size? size);
    }
}
