using ShortDev.Uwp.FullTrust.Xaml;
using SongProjector.Media;
using Windows.UI.Xaml;
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
                if (media is IExternalPresentation externalPresenation)
                {
                    ClearContent();
                    Window.Current.GetSubclass().IsTopMost = false;
                    externalPresenation.Show();
                    externalPresenation.GoToSlide(slideIndex);
                }
                else
                {
                    Window.Current.GetSubclass().IsTopMost = true;
                    RenderContainer.Content = await media.GeneratePresentationAsync(slideIndex, null);
                }
            });
        }

        public void Blank(IMedia media)
        {
            ClearContent();
            Window.Current.GetSubclass().IsTopMost = true;
            if (media is IExternalPresentation externalPresenation)
                externalPresenation.Blank();
        }

        private void ClearContent()
        {
            _ = Dispatcher.RunIdleAsync((x) =>
            {
                RenderContainer.Content = null;
            });
        }
    }
}
