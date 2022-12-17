using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SongProjector.Presentation
{
    public interface IPresentable
    {
        Task<FrameworkElement> GeneratePresentationAsync(RenderContext context);
    }
}
