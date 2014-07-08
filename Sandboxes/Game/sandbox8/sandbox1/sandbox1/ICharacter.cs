using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

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
        protected List<Character> _characters;

        public CharacterManager()
        {
            _characters = new List<Character>();
        }

        public void UpdatePlayerInput(GestureSample[] gestures, Weapon currentWeapon, out GestureSample[] remainingSamples)
        {
            remainingSamples = null;
        }

        public virtual void Update(GameTime gameTime)
        {
            UpdateCharacters(gameTime);
            UpdateSpawners(gameTime);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            short index = 0;
            while (index < _characters.Count)
            {
                _characters[index++].Draw(spriteBatch);
            }
        }

        protected abstract void UpdateCharacters(GameTime gameTime);

        protected abstract void UpdateSpawners(GameTime gameTime);
    }
}
