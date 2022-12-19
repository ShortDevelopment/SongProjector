#nullable enable

using System;
using Windows.UI;
using Windows.UI.Xaml;

namespace SongProjector.Preview;

public sealed class PreviewResult : IDisposable
{
    public string? Title { get; init; }
    public Color? TitleColor { get; init; }

    public FrameworkElement? Content { get; init; }

    public Action? DisposeAction { get; init; }
    public void Dispose()
        => DisposeAction?.Invoke();

    public static implicit operator PreviewResult(FrameworkElement content)
        => new()
        {
            Content = content
        };
}
