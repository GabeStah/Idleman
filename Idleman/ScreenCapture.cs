using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Idleman
{
    /// <summary>
    /// Captures specified region of window into System.Drawing.Bitmap.
    /// </summary>
    /// <see cref="https://gist.github.com/abrkn/3162830"/>
    public static class ScreenCapture
    {
        class NativeMethods
        {
            [DllImport("user32.dll")]
            internal static extern IntPtr GetWindowDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            internal static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("gdi32.dll")]
            internal static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            internal static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]
            internal static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

            [DllImport("gdi32.dll", SetLastError = true)]
            internal static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight,
                IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

            internal const int SRCCOPY = 0x00CC0020;

            [DllImport("gdi32.dll")]
            internal static extern bool DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            internal static extern bool DeleteObject(IntPtr hObject);
        }

        public static Bitmap CaptureWindow(IntPtr handle, Rectangle area)
        {
            var hdcSrc = NativeMethods.GetWindowDC(handle);

            if (hdcSrc == default(IntPtr))
            {
                throw new Exception("Failed to get source window device context. Does it exist?");
            }

            try
            {
                var hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);

                if (hdcDest == default(IntPtr))
                {
                    throw new Exception("Failed to get destination device context.");
                }

                try
                {
                    var hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, area.Width, area.Height);

                    if (hBitmap == default(IntPtr))
                    {
                        throw new Exception("Failed to create get bitmap.");
                    }

                    try
                    {
                        var hOld = NativeMethods.SelectObject(hdcDest, hBitmap);

                        try
                        {
                            if (!NativeMethods.BitBlt(hdcDest, 0, 0, area.Width, area.Height, hdcSrc, area.Left,
                                area.Top, NativeMethods.SRCCOPY))
                            {
                                throw new Exception("BitBlt failed");
                            }

                            return Bitmap.FromHbitmap(hBitmap);
                        }
                        finally
                        {
                            NativeMethods.SelectObject(hdcDest, hOld);
                        }
                    }
                    finally
                    {
                        NativeMethods.DeleteObject(hBitmap);
                    }
                }
                finally
                {
                    NativeMethods.DeleteDC(hdcDest);
                }
            }
            finally
            {
                NativeMethods.ReleaseDC(handle, hdcSrc);
            }
        }
    }
}