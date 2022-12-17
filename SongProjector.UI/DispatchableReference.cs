using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace SongProjector;

internal sealed class DispatchableReference<T>
{
    private DispatchableReference(T reference, CoreDispatcher dispatcher)
    {
        Reference = reference;
        _dispatcher = dispatcher;
    }

    public static DispatchableReference<TObj> Create<TObj>(TObj obj) where TObj : DependencyObject
        => new(obj, obj.Dispatcher);

    public static DispatchableReference<TObj> Create<TObj>(TObj obj, CoreDispatcher dispatcher)
        => new(obj, dispatcher);

    readonly CoreDispatcher _dispatcher;
    public T Reference { get; }

    public void Do(Action<T> action)
        => _ = _dispatcher.RunIdleAsync((x) => action?.Invoke(Reference));

    public async Task<TResult> GetAsync<TResult>(Func<T, TResult> action)
    {
        TaskCompletionSource<TResult> promise = new();
        _ = _dispatcher.RunIdleAsync(_ =>
        {
            try
            {
                promise.SetResult(action.Invoke(Reference));
            }
            catch (Exception ex)
            {
                promise.SetException(ex);
            }
        });
        return await promise.Task;
    }

    public static implicit operator DispatchableReference<T>(T obj)
    {
        if (obj is DependencyObject depObj)
            return new(obj, depObj.Dispatcher);
        return new(obj, CoreWindow.GetForCurrentThread().Dispatcher);
    }

    public static implicit operator T(DispatchableReference<T> @ref)
        => @ref.Reference;
}
