using System.Threading.Tasks;

namespace SongProjector.Preview
{
    public interface IPreviewable
    {
        Task<PreviewResult> GeneratePreviewAsync(RenderContext context);
    }
}
