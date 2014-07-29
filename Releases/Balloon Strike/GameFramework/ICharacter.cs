using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameFramework.Physics;

namespace GameFramework
{
    public interface ICharacter
    {
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch);
        bool Intersects(Vector2 position);
        bool Intersects(Physics.Shapes.Circle circle);
    }

    public abstract class Character : ICharacter
    {
        protected AnimationPlayer _animationPlayer;
        protected Animation _moveAnimation;
        protected Vector2 _positionUL;
        protected Vector2 _positionLR;
        protected Vector2 _velocity;
        protected Physics.Shapes.Rectangle _rectangle;

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

        public bool Intersects(Physics.Shapes.Circle circle)
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

    public abstract class CharacterManager
    {
        protected List<SimpleTimer> Timers
        {
            get;
            set;
        }

        protected List<Character> Characters
        {
            get;
            set;
        }

        protected int ScreenWidth
        {
            get;
            private set;
        }

        protected int ScreenHeight
        {
            get;
            private set;
        }

        private TriggerManager _triggers;

        public CharacterManager(GraphicsDevice graphics, TriggerManager triggers)
        {
            Characters = new List<Character>();
            Timers = new List<SimpleTimer>(5);

            ScreenWidth = graphics.Viewport.Width;
            ScreenHeight = graphics.Viewport.Height;

            _triggers = triggers;

            Initialize();
        }

        public void Update(GameTime gameTime)
        {
            UpdateCharacters(gameTime);
            UpdateSpawners(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            byte index = 0;
            while (index < Characters.Count)
            {
                Characters[index++].Draw(spriteBatch);
            }
        }

        protected void AddTrigger(Trigger newTrigger)
        {
            _triggers.AddTrigger(newTrigger);
        }

        public virtual void UpdatePlayerInput(GestureSample[] gestures, Weapon currentWeapon, out GestureSample[] remainingGestures)
        {
            remainingGestures = gestures;
        }

        protected abstract void Initialize();

        protected abstract void UpdateCharacters(GameTime gameTime);

        protected virtual void UpdateSpawners(GameTime gameTime)
        {
            foreach (SimpleTimer timer in Timers)
            {
                timer.Update(gameTime);
            }
        }
    }
}
