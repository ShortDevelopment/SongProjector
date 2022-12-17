#nullable enable

using System;

namespace SongProjector.Media;

public interface IMediaDisposable : IDisposable
{
    event Action? OnDispose;
}
