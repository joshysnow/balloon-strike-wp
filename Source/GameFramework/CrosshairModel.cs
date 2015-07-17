using System;
using Microsoft.Xna.Framework.Graphics;
using GameCore.Physics.Shapes;

namespace GameFramework
{
    public class CrosshairModel
    {
        public Circle Circle
        {
            get { return _circle; }
        }

        public Texture2D Texture
        {
            get { return _texture; }
        }

        private Circle _circle;
        private Texture2D _texture;

        public CrosshairModel(Circle circle, Texture2D texture)
        {
            _circle = circle;
            _texture = texture;
        }
    }
}
