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

    DispatchableReference<TextBox> _editTextBox;
    public Task<FrameworkElement> GeneratePresentationAsync(int slideId, Size? size)
    {
        if (slideId != 0)
            throw new ArgumentOutOfRangeException();

        _editTextBox = _editTextBox ?? new TextBox();
        _editTextBox.Reference.Foreground = new SolidColorBrush(Colors.White);
        _editTextBox.Reference.FontSize = 40;
        return Task.FromResult<FrameworkElement>(_editTextBox);
    }

    DispatchableReference<TextBlock> _previewTextBlock;
    public Task<FrameworkElement> GeneratePreviewAsync(int slideId)
    {
        if (slideId != 0)
            throw new ArgumentOutOfRangeException();

        _previewTextBlock = _previewTextBlock ?? new TextBlock();
        return Task.FromResult<FrameworkElement>(_previewTextBlock);
    }
}
