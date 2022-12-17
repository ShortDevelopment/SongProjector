#nullable enable

using System;
using Windows.UI.Xaml;

namespace SongProjector.Presentation;

public sealed class PresentationResult
{
    public FrameworkElement? Content { get; init; }

    public Action? DisposeAction { get; init; }
    public void Dispose()
        => DisposeAction?.Invoke();

    public static implicit operator PresentationResult(FrameworkElement content)
        => new()
        {
            Content = content
        };
}
