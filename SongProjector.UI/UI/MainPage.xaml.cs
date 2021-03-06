using SongProjector.Media;
using SongProjector.Presentation;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinUI.Interop.CoreWindow;

namespace SongProjector.UI
{
    public sealed partial class MainPage : Page
    {
        ObservableCollection<MediaReference> collection = new();
        public MainPage()
        {
            this.InitializeComponent();

            ScheduleListView.ItemsSource = collection;
        }

        internal static PresentationManager PresentationManager { get; private set; }

        #region Command Bar
        private async void StartPresentationButton_Click(object sender, RoutedEventArgs e)
        {
            if (PresentationManager == null)
                PresentationManager = await PresentationManager.CreateAsync();
            PresentationManager.Start();

            StartPresentationButton.IsEnabled = false;
            StopPresentationButton.IsEnabled = true;
        }

        private void BlankButton_Click(object sender, RoutedEventArgs e)
        {
            PresentationManager?.Presentation?.Blank();
            PreviewPage.DeselectPreviewItem();
        }

        private void StopPresentationButton_Click(object sender, RoutedEventArgs e)
        {
            PresentationManager.Stop();
            PreviewPage.DeselectPreviewItem();

            StartPresentationButton.IsEnabled = true;
            StopPresentationButton.IsEnabled = false;
        }
        #endregion

        #region MenuBar
        private void ExitMenuButton_Click(object sender, RoutedEventArgs e)
        {
            Window.Current.Close();
        }

        #region Insert
        private async void InsertPdfMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await PickFileAsync(".pdf");
            if (file != null)
            {
                IMedia media = await PdfFileMedia.CreateFromFileAsync(file);
                PreviewPage.CurrentMedia = media;
            }
        }

        private async void InsertPptMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await PickFileAsync(".ppt");
            if (file != null)
            {
                IMedia media = await PdfFileMedia.CreateFromFileAsync(file);
                PreviewPage.CurrentMedia = media;
            }
        }

        private async void InsertImageButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await PickFileAsync(new[] { ".jpg", ".jpeg", ".png" });
            if (file != null)
            {
                IMedia media = await ImageMedia.CreateFromFileAsync(file);
                PreviewPage.CurrentMedia = media;
            }
        }

        private async void InsertVideoButton_Click(object sender, RoutedEventArgs e)
        {
            var file = await PickFileAsync(new[] { ".mov", ".mp4" });
            if (file != null)
            {
                IMedia media = await VideoMedia.CreateFromFileAsync(file);
                PreviewPage.CurrentMedia = media;
            }
        }

        private void InserTextButton_Click(object sender, RoutedEventArgs e)
        {
            IMedia media = new TextMedia();
            PreviewPage.CurrentMedia = media;
        }
        #endregion
#endregion

        async Task<StorageFile> PickFileAsync(string filter)
            => await PickFileAsync(new[] { filter });
        async Task<StorageFile> PickFileAsync(string[] filters)
        {
            FileOpenPicker openPicker = new();
            (openPicker as object as IInitializeWithWindow).Initialize(CoreWindowInterop.CoreWindowHwnd);
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            foreach (var filter in filters)
                openPicker.FileTypeFilter.Add(filter);
            return await openPicker.PickSingleFileAsync();
        }

        public class MediaReference
        {
            public StorageFile File { get; set; }

            public Func<IMedia> CreateMediaCallback { get; set; }
        }
    }
}