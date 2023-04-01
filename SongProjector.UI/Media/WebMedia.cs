#nullable enable

using Microsoft.UI.Xaml.Controls;
using SongProjector.Presentation;
using SongProjector.Preview;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace SongProjector.Media;

internal sealed class WebMedia : MediaBase, IMedia
{
    public WebMedia() : base(file: null) { }
    public override int SlideCount { get; } = 1;

    DispatchableReference<WebView2>? _webviewRef;
    public Task<PresentationResult> GeneratePresentationAsync(RenderContext context)
    {
        if (context.SlideId != 0)
            throw new ArgumentOutOfRangeException();

        WebView2 webView2 = new()
        {
            RequestedTheme = Windows.UI.Xaml.ElementTheme.Default
        };
        _webviewRef = webView2;
        return Task.FromResult<PresentationResult>(webView2);
    }

    DispatchableReference<TextBox>? _previewTextBox;

    public Task<PreviewResult> GeneratePreviewAsync(RenderContext context)
    {
        if (context.SlideId != 0)
            throw new ArgumentOutOfRangeException();

        if (_previewTextBox == null)
        {
            _previewTextBox = new TextBox();
            _previewTextBox.Reference.TextChanged += (s, e) =>
            {
                if (Uri.TryCreate(_previewTextBox.Reference.Text, UriKind.Absolute, out var uri))
                    _webviewRef?.Do((value) => value.Source = uri);
            };
        }
        return Task.FromResult<PreviewResult>(_previewTextBox.Reference);
    }
}
