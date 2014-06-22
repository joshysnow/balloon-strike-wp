using System;
using Microsoft.Xna.Framework;

namespace sandbox7
{
    public static class Collisions
    {
        public static bool Intersects(Vector2 upperLeft, Vector2 lowerRight, Vector2 position)
        {
            return (position.X >= upperLeft.X) && (position.X <= lowerRight.X) &&
                (position.Y >= upperLeft.Y) && (position.Y <= lowerRight.Y);
        }
    }
}
