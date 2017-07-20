using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using GalaSoft.MvvmLight;
using Idleman.Extensions;
using ManagedWinapi.Windows;

namespace Idleman
{
    public enum VirtualKeyCodes : int
    {
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        LButtonDown = 0x0201,
        LButtonUp = 0x0202,
        MK_LBUTTON = 0x0001,
        Zero = 0x00000000
    }

    public partial class Screen : ObservableObject
    {
        private const string ClassNameDefault = "ApolloRuntimeContentWindow";
        private const string WindowNameDefault = "Zombidle";

        private IntPtr _handle;
        public IntPtr Handle
        {
            get => _handle;
            set { Set(() => Handle, ref _handle, value); }
        }

        private Bitmap _screenshot;
        public Bitmap Screenshot
        {
            get => _screenshot;
            set { Set(() => Screenshot, ref _screenshot, value); }
        }

        private SystemWindow _window;
        public SystemWindow Window
        {
            // TODO: Check for ArgumentException on System.Window.Image call.
            get => _window;
            set { Set(() => Window, ref _window, value); }
        }

        private Rect _screenRectangle;
        public Rect ScreenRectangle
        {
            get
            {
                if (_screenRectangle.Width == 0) { UpdateScreenRectangle(); }
                return _screenRectangle;
            }
            set => _screenRectangle = value;
        }

        //If you get 'dllimport unknown'-, then add 'using System.Runtime.InteropServices;'
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string className, string windowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentWindowHandle, IntPtr childWindowHandle, string className, string windowName);
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr windowHandle, VirtualKeyCodes message, int lowParam, int highParam);
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr windowHandle, VirtualKeyCodes message, VirtualKeyCodes lowParam, int highParam);
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr windowHandle, VirtualKeyCodes message, int lowParam, int highParam);
        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr windowHandle, VirtualKeyCodes message, VirtualKeyCodes lowParam, int highParam);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr windowHandle, out Rect lpRect);
        [DllImport("user32.dll", EntryPoint = "PrintWindow")]
        public static extern bool CaptureBitmap(IntPtr windowHandle, IntPtr hdcBlt, int nFlags);

        public Screen()
        {
            Handle = FindWindow(ClassNameDefault, WindowNameDefault);
            Window = new SystemWindow(Handle);
        }

        public Screen(string className)
        {
            Handle = FindWindow(className, "");
        }

        public Screen(string className, string windowName)
        {
            Handle = FindWindow(className, windowName);
        }

        //public Screen(bool useWinApi)
        //{
        //    Handle = FindWindow(ClassNameDefault, WindowNameDefault);
        //    Window = new SystemWindow(Handle);
        //}

        public void TestWinApi()
        {
            
        }

        /// <summary>
        /// Post key press (up + down) to window.
        /// Accepts optional combined coordinates as target.
        /// </summary>
        /// <param name="key">System.Windows.Input.Key to be pressed.</param>
        /// <param name="coordinates">Relative coordinates.</param>
        /// <returns>Indicates if message post was successful.</returns>
        public bool PostKeyPress(Key key, int coordinates)
        {
            // Mouse down.
            var downResult = PostMessage(this.Handle, VirtualKeyCodes.KeyDown, (int)key, coordinates);
            // Mouse up
            var upResult = PostMessage(this.Handle, VirtualKeyCodes.KeyDown, (int)key, coordinates);
            return downResult && upResult;
        }

        /// <summary>
        /// Post key press (up + down) to window.
        /// Accepts optional combined coordinates as target.
        /// </summary>
        /// <param name="key">System.Windows.Input.Key to be pressed.</param>
        /// <param name="x">Relative x-coordinate.</param>
        /// <param name="y">Relative y-coordinate.</param>
        /// <returns>Indicates if message post was successful.</returns>
        public bool PostKeyPress(Key key, int x, int y)
        {
            // Convert coordinates to single int.
            var coordinates = GetCoordinatesParam(x, y);
            // Mouse down.
            var downResult = PostMessage(this.Handle, VirtualKeyCodes.KeyDown, (int)key, coordinates);
            // Mouse up
            var upResult = PostMessage(this.Handle, VirtualKeyCodes.KeyDown, (int)key, coordinates);
            return downResult && upResult;
        }

        /// <summary>
        /// Post mouse click message (up + down) to window.
        /// Accepts optional combined coordinates as target.
        /// </summary>
        /// <param name="coordinates">Relative coordinates.</param>
        /// <returns>Indicates if message post was successful.</returns>
        public bool PostMouseClick(int coordinates)
        {
            // Mouse down.
            var downResult = PostMessage(this.Handle, VirtualKeyCodes.LButtonDown, VirtualKeyCodes.MK_LBUTTON, coordinates);
            // Mouse up
            var upResult = PostMessage(this.Handle, VirtualKeyCodes.LButtonUp, VirtualKeyCodes.Zero, coordinates);
            return downResult && upResult;
        }

        /// <summary>
        /// Post mouse click message (up + down) to window.
        /// Accepts optional coordinates for target (relative) coordinates.
        /// </summary>
        /// <param name="x">Relative x-coordinate.</param>
        /// <param name="y">Relative y-coordinate.</param>
        /// <returns>Indicates if message post was successful.</returns>
        public bool PostMouseClick(int x, int y)
        {
            // Convert coordinates to single int.
            var coordinates = GetCoordinatesParam(x, y);
            // Mouse down.
            var downResult = PostMessage(this.Handle, VirtualKeyCodes.LButtonDown, VirtualKeyCodes.MK_LBUTTON, coordinates);
            // Mouse up
            var upResult = PostMessage(this.Handle, VirtualKeyCodes.LButtonUp, VirtualKeyCodes.Zero, coordinates);
            return downResult && upResult;
        }

        /// <summary>
        /// Convert passed coordinates to single coordinate integer value.
        /// </summary>
        /// <param name="x">X coordinate, starting from top left.</param>
        /// <param name="y">Y coordinate, starting from top left.</param>
        /// <returns>Combined coordinate value for WinAPI param calls.</returns>
        public static int GetCoordinatesParam(int x = 0, int y = 0)
        {
            return (y << 16) | x;
        }

        /// <summary>
        /// Retrieves a bitmap screenshot of current window handle.
        /// </summary>
        /// <remarks>
        /// TODO: Remove GetWindowRect call and use instance variable value, which should be updated on window resize events.
        /// </remarks>
        /// <returns>The retrieved bitmap.</returns>
        public Task<Bitmap> GetBitmap()
        {
            return Task.Run(() => GetBitmapAsync());
        }

        private Bitmap GetBitmapAsync()
        {
            var bitmap = new Bitmap(ScreenRectangle.Width, ScreenRectangle.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bitmap);
            var bitmapHandle = graphics.GetHdc();

            CaptureBitmap(Handle, bitmapHandle, 0);

            graphics.ReleaseHdc(bitmapHandle);
            graphics.Dispose();

            // Delete bitmap handle.
            DeleteObject(bitmapHandle);

            return bitmap;
        }

        /// <summary>
        /// Update the stored ScreenRectangle object for use elsewhere.
        /// </summary>
        private void UpdateScreenRectangle()
        {
            GetWindowRect(Handle, out Rect rectangle);
            ScreenRectangle = rectangle;
        }

        /// <summary>
        /// Get SoftwareBitmap of screen.
        /// </summary>
        /// <returns>SoftwareBitmap image.</returns>
        public async Task<SoftwareBitmap> GetSoftwareBitmap()
        {
            var bitmap = GetBitmap();
            return await bitmap.Result?.ToSoftwareBitmap();
        }
    }
}