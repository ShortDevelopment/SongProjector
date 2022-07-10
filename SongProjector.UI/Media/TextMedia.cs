using SongProjector.Preview;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SongProjector.Media
{
    internal class TextMedia : MediaBase, IMedia
    {
        public TextMedia() : base(null) { }

        public override int SlideCount
            => 1;

        TextBlock _textBlock;
        CoreDispatcher _dispatcher;
        public Task<FrameworkElement> GeneratePresentationAsync(int slideId, Size? size)
        {
            if (slideId != 0)
                throw new ArgumentOutOfRangeException();

            _dispatcher = Window.Current.Dispatcher;

            _textBlock = new();
            _textBlock.Foreground = new SolidColorBrush(Colors.White);
            _textBlock.FontSize = 40;
            return Task.FromResult<FrameworkElement>(_textBlock);
        }

        public Task<FrameworkElement> GeneratePreviewAsync(int slideId)
        {
            if (slideId != 0)
                throw new ArgumentOutOfRangeException();

            TextBox textBox = new();
            textBox.KeyDown += TextBox_KeyDown;
            return Task.FromResult<FrameworkElement>(textBox);
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.F6)
            {
                _ = _dispatcher?.RunIdleAsync((x) =>
                {
                    _textBlock.Text = ((TextBox)sender).Text;
                });
            }
        }
    }
}
