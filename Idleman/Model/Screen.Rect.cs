using System.Drawing;
using System.Runtime.InteropServices;
using GalaSoft.MvvmLight;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace Idleman
{
    partial class Screen : ObservableObject
    {
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
    }
}