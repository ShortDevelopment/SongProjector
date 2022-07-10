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

        public Task<FrameworkElement> GeneratePresentationAsync(int slideId, Size? size)
        {
            if (slideId != 0)
                throw new ArgumentOutOfRangeException();

            Image result = new();
            result.Source = new BitmapImage(new Uri(File.Path));
            return Task.FromResult<FrameworkElement>(result);
        }

        public async Task<FrameworkElement> GeneratePreviewAsync(int slideId)
            => await GeneratePresentationAsync(slideId, null);
    }
}
