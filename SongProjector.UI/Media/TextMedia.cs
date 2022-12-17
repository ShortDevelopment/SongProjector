#nullable enable

using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace SongProjector.Media;

internal class TextMedia : MediaBase, IMedia
{
    public TextMedia() : base(null) { }

    public override int SlideCount
        => 1;

    DispatchableReference<TextBlock>? _editTextBlock;
    public Task<FrameworkElement> GeneratePresentationAsync(int slideId, Size? size)
    {
        if (slideId != 0)
            throw new ArgumentOutOfRangeException();

        _editTextBlock = _editTextBlock ?? new TextBlock();
        _editTextBlock.Reference.Foreground = new SolidColorBrush(Colors.White);
        _editTextBlock.Reference.FontSize = 40;
        return Task.FromResult<FrameworkElement>(_editTextBlock);
    }

    DispatchableReference<TextBox>? _previewTextBox;
    public Task<FrameworkElement> GeneratePreviewAsync(int slideId)
    {
        if (slideId != 0)
            throw new ArgumentOutOfRangeException();

        if (_previewTextBox == null)
        {
            _previewTextBox = new TextBox();
            _previewTextBox.Reference.TextChanged += (s, e) =>
            {
                _editTextBlock?.Do((value) => value.Text = _previewTextBox.Reference.Text);
            };
        }
        return Task.FromResult<FrameworkElement>(_previewTextBox);
    }
}
