using System;
using System.Runtime.InteropServices;
using Windows.UI.Xaml;
using WinUI.Interop.CoreWindow;

namespace ShortDev.Windows.Uwp.Presentation
{
    public class AeroPeak
    {
        public static void DisableForWindow(Window window)
            => DisableForHwnd(window.CoreWindow.GetHwnd());

        public static void DisableForHwnd(IntPtr hwnd)
        {
            bool value = true;
            Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(hwnd, DWMWA_DISALLOW_PEEK, ref value, Marshal.SizeOf<int>()));
            Marshal.ThrowExceptionForHR(DwmSetWindowAttribute(hwnd, DWMWA_EXCLUDED_FROM_PEEK, ref value, Marshal.SizeOf<int>()));
        }

        const uint DWMWA_DISALLOW_PEEK = 11;
        const uint DWMWA_EXCLUDED_FROM_PEEK = 12;

        [DllImport("dwmapi.dll", PreserveSig = true)]
        static extern int DwmSetWindowAttribute(IntPtr hwnd, uint dwAttribute, ref bool pvAttribute, int cbAttribute);

        enum DWMWINDOWATTRIBUTE
        {
            DWMWA_NCRENDERING_ENABLED,
            DWMWA_NCRENDERING_POLICY,
            DWMWA_TRANSITIONS_FORCEDISABLED,
            DWMWA_ALLOW_NCPAINT,
            DWMWA_CAPTION_BUTTON_BOUNDS,
            DWMWA_NONCLIENT_RTL_LAYOUT,
            DWMWA_FORCE_ICONIC_REPRESENTATION,
            DWMWA_FLIP3D_POLICY,
            DWMWA_EXTENDED_FRAME_BOUNDS,
            DWMWA_HAS_ICONIC_BITMAP,
            DWMWA_DISALLOW_PEEK,
            DWMWA_EXCLUDED_FROM_PEEK,
            DWMWA_CLOAK,
            DWMWA_CLOAKED,
            DWMWA_FREEZE_REPRESENTATION,
            DWMWA_PASSIVE_UPDATE_MODE,
            DWMWA_USE_HOSTBACKDROPBRUSH,
            DWMWA_USE_IMMERSIVE_DARK_MODE = 20,
            DWMWA_WINDOW_CORNER_PREFERENCE = 33,
            DWMWA_BORDER_COLOR,
            DWMWA_CAPTION_COLOR,
            DWMWA_TEXT_COLOR,
            DWMWA_VISIBLE_FRAME_BORDER_THICKNESS,
            DWMWA_SYSTEMBACKDROP_TYPE,
            DWMWA_LAST
        };
    }
}
