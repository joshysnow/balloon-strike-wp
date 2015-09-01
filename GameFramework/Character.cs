using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameCore.Physics;

namespace GameFramework
{
    public abstract class Character
    {
        public Vector2 PositionUL
        {
            get { return _positionUL; }
            set { _positionUL = value; }
        }

        public Vector2 PositionLR
        {
            get { return _positionLR; }
            set { _positionLR = value; }
        }

        protected AnimationPlayer _animationPlayer;
        protected Vector2 _positionUL;
        protected Vector2 _positionLR;
        protected Vector2 _velocity;
        protected GameCore.Physics.Shapes.Rectangle _rectangle;

        public Character()
        {
            _animationPlayer = new AnimationPlayer();
        }

        public virtual void Update(GameTime gameTime)
        {
            _animationPlayer.Update(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            _animationPlayer.Draw(spriteBatch);
        }

        public bool Intersects(Vector2 position)
        {
            return Collisions.Intersects(_positionUL, _positionLR, position);
        }

        public bool Intersects(GameCore.Physics.Shapes.Circle circle)
        {
            return Collisions.Circle_Rectangle(circle, _rectangle);
        }

        protected void UpdatePosition()
        {
            _positionUL += _velocity;
            _positionLR += _velocity;

            _rectangle.Update(_positionUL, _positionLR);
        }
    }
}
