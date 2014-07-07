using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox8
{
    public interface ICharacter
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        bool Intersects(Vector2 position, float radius);
    }

    public abstract class Character : ICharacter
    {
        protected AnimationPlayer _animationPlayer;
        protected Animation _moveAnimation;
        protected Animation _popAnimation;
        protected Vector2 _positionUL;
        protected Vector2 _positionLR;
        protected Vector2 _velocity;

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

        public bool Intersects(Vector2 position, float radius)
        {
            return Collisions.Intersects(_positionUL, _positionLR, position) || 
                Collisions.Intersects(_positionUL, _positionLR, position, radius);
        }
    }

    public abstract class CharacterManager
    {

    }
}
