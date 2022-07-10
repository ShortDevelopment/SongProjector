using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SongProjector.Preview
{
    public interface IPreviewable
    {
        Task<FrameworkElement> GeneratePreviewAsync(int slideId);
    }
}
