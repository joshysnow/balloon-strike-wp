using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;
using GameCore.Timers;
using GameCore.Physics;
using GameCore.Triggers;

namespace GameFramework
{
    public abstract class CharacterManager : Serializable
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

        private TriggerManager _triggerManager;

        public CharacterManager(GraphicsDevice graphics)
        {
            Characters = new List<Character>();
            Timers = new List<SimpleTimer>(5);

            ScreenWidth = graphics.Viewport.Width;
            ScreenHeight = graphics.Viewport.Height;

            _triggerManager = new TriggerManager();
        }

        public abstract void Initialize();

        public abstract void Activate(bool instancePreserved);

        public abstract void Deactivate();

        public virtual void Update(GameTime gameTime)
        {
            UpdateCharacters(gameTime);
            UpdateSpawners(gameTime);
            UpdateTriggers(gameTime);
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
            _triggerManager.AddTrigger(newTrigger);
        }

        public virtual void UpdatePlayerInput(GestureSample[] gestures, Weapon currentWeapon, out GestureSample[] remainingGestures)
        {
            remainingGestures = gestures;
        }

        protected abstract void UpdateCharacters(GameTime gameTime);

        private void UpdateSpawners(GameTime gameTime)
        {
            foreach (SimpleTimer timer in Timers)
            {
                timer.Update(gameTime);
            }
        }

        private void UpdateTriggers(GameTime gameTime)
        {
            _triggerManager.Update(gameTime);
        }
    }
}
