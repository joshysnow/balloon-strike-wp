using System;
using Microsoft.Xna.Framework;
using GameFramework.Physics.Shapes;

namespace GameFramework.Physics
{
    public class Collisions
    {
        public static bool Intersects(Vector2 upperLeft, Vector2 lowerRight, Vector2 position)
        {
            return (position.X >= upperLeft.X) && (position.X <= lowerRight.X) &&
                (position.Y >= upperLeft.Y) && (position.Y <= lowerRight.Y);
        }

        public static bool Circle_Point(Circle circle, Vector2 point)
        {
            Vector2 distance = Vector2.Subtract(circle.Center, point);
            float length = Math.Abs(distance.Length());

            return length <= circle.Radius;
        }

        public static bool Circle_Line(Circle circle, Line line)
        {
            // Get the distance between the centre of the circle and the base of the line.
            Vector2 baseDistance = Vector2.Subtract(circle.Center, line.Base);

            // Project this onto the line.
            Vector2 projection = Project_Vector2(baseDistance, line.Direction);

            // Then deduce the nearest point on the line from the distance projected onto the line.
            Vector2 nearest = Vector2.Add(line.Base, projection);

            return Circle_Point(circle, nearest);
        }

        public static bool Circle_Segment(Circle circle, LineSegment segment)
        {
            // Test if either end of the segment is inside the circle.
            if (Circle_Point(circle, segment.PointA))
                return true;
            if (Circle_Point(circle, segment.PointB))
                return true;

            // Test if the line goes through the circle.
            Vector2 segmentDistance = Vector2.Subtract(segment.PointB, segment.PointA);
            Vector2 baseDistance = Vector2.Subtract(circle.Center, segment.PointA);
            Vector2 projection = Project_Vector2(baseDistance, segment.PointB);
            Vector2 nearest = Vector2.Add(segment.PointA, projection);

            return Circle_Point(circle, nearest) && 
                projection.LengthSquared() <= segmentDistance.LengthSquared() &&
                0 <= Vector2.Dot(projection, segmentDistance);
        }

        public static bool Circle_Rectangle(Circle circle, GameFramework.Physics.Shapes.Rectangle rectangle)
        {
            Vector2 clamped = Vector2.Clamp(circle.Center, rectangle.TopLeft, rectangle.BottomRight);
            return Circle_Point(circle, clamped);
        }

        public static bool Rectangle_Point(GameFramework.Physics.Shapes.Rectangle rectangle, Vector2 point)
        {
            bool insideXBounds = (point.X >= rectangle.TopLeft.X) && (point.X <= rectangle.TopRight.X);
            bool insideYBounds = (point.Y >= rectangle.TopLeft.Y) && (point.Y <= rectangle.BottomLeft.Y);

            return insideXBounds && insideYBounds;
        }

        private static Vector2 Project_Vector2(Vector2 project, Vector2 to)
        {
            return Vector2.Multiply(project, Vector2.Dot(project, to) / Vector2.Dot(to, to));
        }
    }
}
