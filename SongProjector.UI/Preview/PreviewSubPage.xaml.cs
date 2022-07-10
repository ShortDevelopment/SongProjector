using SongProjector.Media;
using SongProjector.UI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SongProjector.Preview
{
    public sealed partial class PreviewSubPage : Page
    {
        public PreviewSubPage()
        {
            this.InitializeComponent();
        }

        IMedia _currentMedia;
        internal IMedia CurrentMedia
        {
            get => _currentMedia;
            set
            {
                _currentMedia = value;
                GeneratePreviewItems();
            }
        }

        public ObservableCollection<PreviewItemControl> PreviewItems { get; } = new();
        async void GeneratePreviewItems()
        {
            PreviewItems.Clear();

            await Task.WhenAll(Enumerable.Range(0, CurrentMedia.SlideCount).Select(async (i) =>
            {
                PreviewItems.Add(new()
                {
                    Title = $"Slide {i + 1}",
                    PreviewContent = await CurrentMedia.GeneratePreviewAsync(i)
                });
            }));
        }

        private void SelectionGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var previewItem = (PreviewItemControl)SelectionGridView.SelectedItem;
            if (previewItem == null)
                return;

            MainPage.PresentationManager?.Presentation?.Present(CurrentMedia, SelectionGridView.SelectedIndex);
        }

        public void DeselectPreviewItem()
        {
            SelectionGridView.SelectedItem = null;
        }
    }
}
