using ShortDev.Uwp.FullTrust.Core.Xaml;
using System.Threading;
using Windows.UI.Core;

namespace SongProjector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new XamlApplicationWrapper(() => null); // Dummy instance to make api happy
            var app = new App();
            Thread mainWindowThread = new(() =>
            {
                var window = XamlWindowActivator.CreateNewWindow(new("SongProjector"));
                app.LaunchInternal();
                window.CoreWindow.Dispatcher.ProcessEvents(CoreProcessEventsOption.ProcessUntilQuit);
            });
            mainWindowThread.Start();
            mainWindowThread.Join();
        }
    }
}
