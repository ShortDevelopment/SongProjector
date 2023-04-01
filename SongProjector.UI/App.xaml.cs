using Microsoft.UI.Xaml.Controls;
using SongProjector.UI;
using System;
using System.Runtime.InteropServices;
using System.Text;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinUI.Interop.CoreWindow;

namespace SongProjector
{
    sealed partial class App : Application
    {
        public App()
        {
            // this.InitializeComponent();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.UnhandledException += App_UnhandledException;
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs e)
            => LaunchInternal();

        internal void LaunchInternal()
        {
            Window window = Window.Current;

            Frame rootFrame = window.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                window.Content = rootFrame;
            }

            if (rootFrame.Content == null)
                rootFrame.Navigate(typeof(MainPage));

            FixWin2D(window);

            window.Activate();
        }

        public static void FixWin2D(Window window)
        {
            var hwnd = window.CoreWindow.GetHwnd();
            PostMessage(hwnd, 0x270, 0, 1);
        }

        [DllImport("User32.Dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
