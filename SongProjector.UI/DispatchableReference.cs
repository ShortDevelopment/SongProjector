using System;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SongProjector;

internal sealed class DispatchableReference<T>
{
    private DispatchableReference(T reference, CoreDispatcher dispatcher)
    {
        Reference = reference;
        Dispatcher = dispatcher;
    }

    public static DispatchableReference<TObj> Create<TObj>(TObj obj) where TObj : DependencyObject
        => new(obj, obj.Dispatcher);

    public static DispatchableReference<TObj> Create<TObj>(TObj obj, CoreDispatcher dispatcher)
        => new(obj, dispatcher);

    public CoreDispatcher Dispatcher { get; }
    public T Reference { get; }

    public void Do(Action<T> action)
        => _ = Dispatcher.RunIdleAsync((x) => action?.Invoke(Reference));

    public static implicit operator DispatchableReference<T>(T obj)
    {
        if (obj is DependencyObject depObj)
            return new(obj, depObj.Dispatcher);
        return new(obj, CoreWindow.GetForCurrentThread().Dispatcher);
    }

    public static implicit operator T(DispatchableReference<T> @ref)
        => @ref.Reference;
}
