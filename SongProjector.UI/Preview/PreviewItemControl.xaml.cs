using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SongProjector.Preview
{
    public sealed partial class PreviewItemControl : UserControl
    {
        public PreviewItemControl()
        {
            this.InitializeComponent();
        }

        public FrameworkElement PreviewContent
        {
            get => (FrameworkElement)PreviewPresenter.Content;
            set => PreviewPresenter.Content = value;
        }

        public string Title
        {
            get => TitlePreviewTextBlock.Text;
            set => TitlePreviewTextBlock.Text = value;
        }
    }
}
