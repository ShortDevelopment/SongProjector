using System;
using Windows.UI.Core;

namespace SongProjector
{
    internal class DispatchableReference<T>
    {
        public DispatchableReference(CoreDispatcher dispatcher, T reference)
        {
            Dispatcher = dispatcher;
            Reference = reference;
        }

        public CoreDispatcher Dispatcher { get; }
        public T Reference { get; }

        public void BeginInvoke(Action<T> action)
            => _ = Dispatcher.RunIdleAsync((x) => action?.Invoke(Reference));
    }
}
