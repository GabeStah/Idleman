namespace Idleman
{
    /// <summary>
    /// Used to specify attachment position for shape objects.
    /// </summary>
    internal class Anchor
    {
        public enum Corners
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public enum Sides
        {
            Top,
            Bottom,
            Left,
            Right
        }

        public Corners Corner { get; set; }

        public Sides Side { get; set; }

        public Anchor(Corners corner)
        {
            Corner = corner;
        }

        public Anchor(Sides side)
        {
            Side = side;
        }
    }
}
