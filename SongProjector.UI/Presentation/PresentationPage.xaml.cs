using SongProjector.Media;
using Windows.UI.Xaml.Controls;

namespace SongProjector.Presentation
{
    public sealed partial class PresentationPage : Page, IPresentation
    {
        public PresentationPage()
        {
            this.InitializeComponent();
        }

        public void Present(IMedia media, int slideIndex)
        {
            _ = Dispatcher.RunIdleAsync(async (x) =>
            {
                RenderContainer.Content = await media.GeneratePresentationAsync(slideIndex, null);
            });
        }

        public void Blank()
        {
            _ = Dispatcher.RunIdleAsync((x) =>
            {
                RenderContainer.Content = null;
            });
        }
    }
}
