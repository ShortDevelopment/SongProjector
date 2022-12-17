using SongProjector.Preview;
using System;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SongProjector.Media
{
    internal class PdfFileMedia : MediaBase, IMedia
    {
        PdfDocument _doc;
        private PdfFileMedia(StorageFile file, PdfDocument doc) : base(file)
        {
            _doc = doc;
        }

        public static async Task<PdfFileMedia> CreateFromFileAsync(StorageFile file)
        {
            var doc = await PdfDocument.LoadFromFileAsync(file);
            return new(file, doc);
        }

        public override int SlideCount
            => (int)_doc.PageCount;

        public async Task<FrameworkElement> GeneratePresentationAsync(RenderContext context)
        {
            var slideId = context.SlideId;
            if (slideId < 0 || slideId > _doc.PageCount)
                throw new ArgumentOutOfRangeException();

            Image result = new();
            BitmapImage img = new();
            using (var page = _doc.GetPage((uint)slideId))
            using (var stream = new InMemoryRandomAccessStream())
            {
                await page.RenderToStreamAsync(stream);
                await img.SetSourceAsync(stream);
            }
            result.Source = img;
            return result;
        }

        public async Task<PreviewResult> GeneratePreviewAsync(RenderContext context)
            => await GeneratePresentationAsync(context);
    }
}
