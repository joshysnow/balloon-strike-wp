using System;
using Microsoft.Xna.Framework;

namespace GameFramework
{
    public class Collisions
    {
        public static bool Intersects(Vector2 upperLeft, Vector2 lowerRight, Vector2 position)
        {
            return (position.X >= upperLeft.X) && (position.X <= lowerRight.X) &&
                (position.Y >= upperLeft.Y) && (position.Y <= lowerRight.Y);
        }

        public static bool Intersects(Vector2 upperLeft, Vector2 lowerRight, Vector2 position, float radius)
        {
            Vector2 upperRight = new Vector2(upperLeft.Y, lowerRight.X);
            Vector2 lowerLeft = new Vector2(upperLeft.X, lowerRight.Y);

            return Math.Abs(Vector2.Subtract(upperLeft, position).Length()) <= radius ||
                   Math.Abs(Vector2.Subtract(lowerRight, position).Length()) <= radius ||
                   Math.Abs(Vector2.Subtract(upperRight, position).Length()) <= radius ||
                   Math.Abs(Vector2.Subtract(lowerLeft, position).Length()) <= radius;
        }
    }
}
