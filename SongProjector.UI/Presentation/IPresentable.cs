using System.Threading.Tasks;

namespace SongProjector.Presentation;

public interface IPresentable
{
    Task<PresentationResult> GeneratePresentationAsync(RenderContext context);
}
