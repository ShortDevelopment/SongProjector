﻿#nullable enable

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

        IMedia? _currentMedia;
        internal IMedia? CurrentMedia
        {
            get => _currentMedia;
            set
            {
                _currentMedia = value;
                GeneratePreviewItems();
            }
        }

        async void GeneratePreviewItems()
        {
            ObservableCollection<PreviewItemControl> previewItems = new();
            SelectionGridView.ItemsSource = previewItems;

            if (CurrentMedia == null)
                return;

            var items = await Task.WhenAll(Enumerable.Range(0, CurrentMedia.SlideCount).Select(async (i) =>
            {
                var preview = await CurrentMedia.GeneratePreviewAsync(new()
                {
                    SlideId = i,
                    Size = new(200, 112.5)
                });
                return new PreviewItemControl()
                {
                    Index = i,
                    Title = preview.Title ?? $"Slide {i + 1}",
                    TitleColor = preview.TitleColor,
                    PreviewContent = preview.Content
                };
            }));
            foreach (var item in items.OrderBy((x) => x.Index))
                previewItems.Add(item);
        }

        private void SelectionGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var previewItem = (PreviewItemControl)SelectionGridView.SelectedItem;
            if (previewItem == null)
                return;

            MainPage.PresentationManagers.ForEach(p => p.Presentation?.Present(CurrentMedia, SelectionGridView.SelectedIndex));
        }

        public void DeselectPreviewItem()
        {
            SelectionGridView.SelectedItem = null;
        }
    }
}
