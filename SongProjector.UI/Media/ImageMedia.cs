using SongProjector.Preview;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace SongProjector.Media
{
    internal class ImageMedia : MediaBase, IMedia
    {
        public ImageMedia(StorageFile file) : base(file) { }

        public static Task<ImageMedia> CreateFromFileAsync(StorageFile file)
            => Task.FromResult(new ImageMedia(file));

        public override int SlideCount
            => 1;

        public Task<FrameworkElement> GeneratePresentationAsync(RenderContext context)
        {
            if (context.SlideId != 0)
                throw new ArgumentOutOfRangeException();

            Image result = new();
            result.Source = new BitmapImage(new Uri(File.Path));
            return Task.FromResult<FrameworkElement>(result);
        }

        public async Task<PreviewResult> GeneratePreviewAsync(RenderContext context)
            => await GeneratePresentationAsync(context);
    }
}
