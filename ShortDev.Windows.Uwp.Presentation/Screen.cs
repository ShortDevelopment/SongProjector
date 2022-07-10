using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Windows.Foundation;

namespace ShortDev.Windows.Uwp.Presentation
{
    public sealed class Screen
    {
        #region GetScreensAsync
        private class ScreenEnumerator
        {
            public static IReadOnlyList<Screen> Enumerate()
            {
                List<Screen> screens = new();
                MonitorEnumProc callback = (IntPtr hMonitor, IntPtr hdc, IntPtr rect, IntPtr lparam) =>
                {
                    screens.Add(new(hMonitor, hdc));
                    return true;
                };
                EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero);
                return screens;
            }

            [DllImport("User32.dll")]
            static extern bool EnumDisplayMonitors(IntPtr lpDevice, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);

            delegate bool MonitorEnumProc(IntPtr hmonitor, IntPtr hdc, IntPtr rect, IntPtr lparam);
        }

        public static Task<IReadOnlyList<Screen>> GetScreensAsync()
            => Task.FromResult(ScreenEnumerator.Enumerate());
        #endregion

        #region Bounds
        public bool IsPrimary { get; }

        public string DisplayName { get; }

        public Rect Bounds { get; }
        #endregion

        private unsafe Screen(IntPtr hMonitor, IntPtr hdc)
        {
            MONITORINFOEXW info = new()
            {
                cbSize = (uint)sizeof(MONITORINFOEXW)
            };
            GetMonitorInfoW(hMonitor, ref info);

            var _bounds = info.rcMonitor;
            Bounds = new(_bounds.left, _bounds.top, _bounds.right - _bounds.left, _bounds.bottom - _bounds.top);
            DisplayName = new string(info.szDevice);

            const int MONITORINFOF_PRIMARY = 0x00000001;
            IsPrimary = (info.dwFlags & MONITORINFOF_PRIMARY) != 0;
        }

        #region Monitor Info
        [DllImport("user32.dll")]
        static extern bool GetMonitorInfoW(IntPtr hMonitor, ref MONITORINFOEXW lpmi);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        unsafe struct MONITORINFOEXW
        {
            public uint cbSize;
            public Win32_Rect rcMonitor;
            public Win32_Rect rcWork;
            public int dwFlags;
            public fixed char szDevice[32];
        }

        [StructLayout(LayoutKind.Sequential)]
        struct Win32_Rect
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        #endregion

        //public static void Test()
        //{
        //    ManagementObjectSearcher searcher = new("SELECT * FROM Win32_DesktopMonitor");

        //    foreach (ManagementObject queryObj in searcher.Get())
        //    {
        //        Debug.WriteLine("-----------------------------------");
        //        Debug.WriteLine($"Name: {queryObj["Name"]}");
        //        Debug.WriteLine($"DeviceID: {queryObj["DeviceID"]}");
        //        Debug.WriteLine($"MonitorType: {queryObj["MonitorType"]}");
        //        Debug.WriteLine($"Name: {queryObj["Name"]}");
        //    }
        //}
    }
}
