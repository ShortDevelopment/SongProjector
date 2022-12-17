using ShortDev.Uwp.FullTrust.Xaml;
using ShortDev.Windows.Uwp.Presentation;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Devices.Display;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.UI.Xaml;
using WinUI.Interop.CoreWindow;

namespace SongProjector.Presentation
{
    internal class PresentationManager
    {
        // static DisplayManager _displayManager = DisplayManager.Create(DisplayManagerOptions.None);
        public static async Task<DisplayMonitor[]> GetMonitorsAsync()
        {
            var devices = await DeviceInformation.FindAllAsync(DisplayMonitor.GetDeviceSelector());
            return await Task.WhenAll(devices.Select(async (device) => await DisplayMonitor.FromInterfaceIdAsync(device.Id)));
        }

        Rect _displayBounds;
        private PresentationManager(Rect displayBounds)
        {
            _displayBounds = displayBounds;
        }

        public static async Task<PresentationManager> CreateAsync()
            => await CreateForScreenAsync((await Screen.GetScreensAsync()).Where((x) => !x.IsPrimary).First());

        public static Task<PresentationManager> CreateForScreenAsync(Screen screen)
            => Task.FromResult(new PresentationManager(screen.Bounds));

        PresentationPage? _presentation;
        public IPresentation? Presentation
            => _presentation;

        Window _window;
        public void Start()
        {
            if (_window != null)
                throw new InvalidOperationException("Presentation already running!");

            IntPtr mainWindowHwnd = CoreWindowInterop.CoreWindowHwnd;
            FullTrustApplication.CreateNewUIThread(() =>
            {
                _window = XamlWindowActivator.CreateNewWindow(new("Presentation")
                {
                    HasWin32TitleBar = false,
                    HasWin32Frame = false,
                    IsTopMost = true
                });
                App.FixWin2D(_window);
                _window.Activate();

                _presentation = new();
                _window.Content = _presentation;

                AeroPeak.DisableForWindow(_window);
                
                // _window.GetSubclass().WindowPrivate.MoveWindow((int)_displayBounds.X, (int)_diplayBounds.Y, (int)_displayBounds.Width, (int)_displayBounds.Height);
                const int SWP_NOACTIVATE = 0x0010;
                const int SWP_NOZORDER = 0x0004;
                SetWindowPos(_window.GetHwnd(), IntPtr.Zero, (int)_displayBounds.X, (int)_displayBounds.Y, (int)_displayBounds.Width, (int)_displayBounds.Height, SWP_NOACTIVATE | SWP_NOZORDER);

                SetForegroundWindow(mainWindowHwnd);

                _window.Dispatcher.ProcessEvents(Windows.UI.Core.CoreProcessEventsOption.ProcessUntilQuit);
            });
        }

        public void Stop()
        {
            if (_window == null)
                throw new InvalidOperationException("Presentation not running!");

            var windowRef = _window;
            _ = windowRef.Dispatcher.RunIdleAsync((x) => windowRef.Close());

            _window = null;
            _presentation = null;
        }

        public static async void Test()
        {
            var monitors = await GetMonitorsAsync();
        }

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int width, int height, uint flags);
    }
}
