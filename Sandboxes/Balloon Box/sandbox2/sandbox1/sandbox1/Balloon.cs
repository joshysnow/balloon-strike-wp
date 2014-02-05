using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox2
{
    public enum BalloonColour : byte
    {
        Red = 0x01,
        Green = 0x02,
        Blue = 0x04
    }

    public class Balloon
    {
        private Texture2D _texture;

        public BalloonColour Colour
        {
            get; set;
        }

        private Vector2 _position;
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        private Vector2 _velocity;
        public Vector2 Velocity
        {
            get
            {
                return _velocity;
            }
            set
            {
                _velocity = value;
            }
        }

        public string Name
        {
            get; private set;
        }

        public Balloon(string name, ref Texture2D texture)
        {
            this.Name = name;
            _texture = texture;
            _position = new Vector2();
        }

        public void SetPosition(float x, float y)
        {
            _position.X = x;
            _position.Y = y;
        }

        public void Update(GameTime gameTime)
        {
            Position += Velocity;
        }

        public void Draw(ref SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, Color.White);
        }
    }
}
