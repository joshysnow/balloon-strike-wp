using Microsoft.Xna.Framework;

namespace GameFramework.Physics.Shapes
{
    public class Rectangle
    {
        public LineSegment Top
        {
            get { return new LineSegment() { PointA = TopLeft, PointB = BottomRight }; }
        }

        public LineSegment Right
        {
            get { return new LineSegment() { PointA = TopRight, PointB = BottomRight }; }
        }

        public LineSegment Left
        {
            get { return new LineSegment() { PointA = TopLeft, PointB = BottomLeft }; }
        }

        public LineSegment Bottom
        {
            get { return new LineSegment() { PointA = BottomLeft, PointB = BottomRight }; }
        }

        public Vector2 TopLeft
        {
            get;
            private set;
        }

        public Vector2 TopRight
        {
            get;
            private set;
        }

        public Vector2 BottomLeft
        {
            get;
            private set;
        }

        public Vector2 BottomRight
        {
            get;
            private set;
        }

        public Rectangle(Vector2 topLeft, Vector2 bottomRight)
        {
            TopLeft = topLeft;
            BottomRight = bottomRight;

            TopRight = new Vector2(bottomRight.X, topLeft.Y);
            BottomLeft = new Vector2(topLeft.X, bottomRight.Y);
        }

        public Rectangle(int x, int y, int width, int height)
        {
            TopLeft = new Vector2(x, y);
            TopRight = new Vector2(x + width, y);
            BottomLeft = new Vector2(x, y + height);
            BottomRight = new Vector2(x + width, y + height);
        }

        public void Update(Vector2 newTopLeft, Vector2 newBottomRight)
        {
            TopLeft = newTopLeft;
            BottomRight = newBottomRight;

            TopRight = new Vector2(newBottomRight.X, newTopLeft.Y);
            BottomLeft = new Vector2(newTopLeft.X, newBottomRight.Y);
        }
    }
}
