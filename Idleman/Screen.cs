using System;
using System.Collections.Generic;
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
using Idleman.Extensions;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Idleman
{
    internal enum VirtualKeyCodes : int
    {
        KeyDown = 0x0100,
        KeyUp = 0x0101,
        LButtonDown = 0x0201,
        LButtonUp = 0x0202,
        MK_LBUTTON = 0x0001,
        Zero = 0x00000000
    }

    class Screen
    {
        public Bitmap Screenshot { get; set;}
        public IntPtr Handle { get; set; }

        private const string ClassNameDefault = "ApolloRuntimeContentWindow";
        private const string WindowNameDefault = "Zombidle";

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

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public Rect(Rect rectangle) : this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
            {
            }
            public Rect(int left, int top, int right, int bottom)
            {
                X = left;
                Y = top;
                this.Right = right;
                this.Bottom = bottom;
            }

            public int X { get; set; }

            public int Y { get; set; }

            public int Left
            {
                get => X;
                set => X = value;
            }
            public int Top
            {
                get => Y;
                set => Y = value;
            }
            public int Right { get; set; }

            public int Bottom { get; set; }

            public int Height
            {
                get => Bottom - Y;
                set => Bottom = value + Y;
            }
            public int Width
            {
                get => Right - X;
                set => Right = value + X;
            }
            public Point Location
            {
                get => new Point(Left, Top);
                set
                {
                    X = (int) value.X;
                    Y = (int) value.Y;
                }
            }
            public Size Size
            {
                get => new Size(Width, Height);
                set
                {
                    Right = (int) (value.Width + X);
                    Bottom = (int) (value.Height + Y);
                }
            }

            public static implicit operator Rectangle(Rect rectangle)
            {
                return new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
            }
            public static implicit operator Rect(Rectangle rectangle)
            {
                return new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }
            public static bool operator ==(Rect rectangle1, Rect rectangle2)
            {
                return rectangle1.Equals(rectangle2);
            }
            public static bool operator !=(Rect rectangle1, Rect rectangle2)
            {
                return !rectangle1.Equals(rectangle2);
            }

            public override string ToString()
            {
                return "{Left: " + X + "; " + "Top: " + Y + "; Right: " + Right + "; Bottom: " + Bottom + "}";
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public bool Equals(Rect rectangle)
            {
                return rectangle.Left == X && rectangle.Top == Y && rectangle.Right == Right && rectangle.Bottom == Bottom;
            }

            public override bool Equals(object Object)
            {
                if (Object is Rect)
                {
                    return Equals((Rect)Object);
                }
                else if (Object is Rectangle)
                {
                    return Equals(new Rect((Rectangle)Object));
                }

                return false;
            }
        }

        public Screen()
        {
            Handle = FindWindow(ClassNameDefault, WindowNameDefault);
        }

        public Screen(string className)
        {
            Handle = FindWindow(className, "");
        }

        public Screen(string className, string windowName)
        {
            Handle = FindWindow(className, windowName);
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
        public Bitmap GetBitmap()
        {
            GetWindowRect(this.Handle, out Rect rect);

            var bitmap = new Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(bitmap);
            var bitmapHandle = graphics.GetHdc();

            CaptureBitmap(this.Handle, bitmapHandle, 0);

            graphics.ReleaseHdc(bitmapHandle);
            graphics.Dispose();

            // Delete bitmap handle.
            DeleteObject(bitmapHandle);

            // Set current screenshot.
            this.Screenshot = bitmap;

            return bitmap;
        }

        /// <summary>
        /// Get SoftwareBitmap of screen.
        /// </summary>
        /// <returns>SoftwareBitmap image.</returns>
        public async Task<SoftwareBitmap> GetSoftwareBitmap()
        {
            return await GetBitmap().ToSoftwareBitmap();
        }

        public void SaveScreenshot(string filePath)
        {
            Screenshot.Save(filePath, ImageFormat.Png);
        }
    }
}