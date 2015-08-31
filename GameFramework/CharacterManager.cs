using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;
using GameCore.Timers;
using GameCore.Physics;
using GameCore.Triggers;
using GameFramework.Triggers;

namespace GameFramework
{
    public abstract class CharacterManager : Serializable
    {
        protected List<SimpleTimer> Timers
        {
            get { return _timers; }
        }

        protected List<Character> Characters
        {
            get { return _characters; }
        }

        protected TriggerManager Triggers
        {
            get { return _triggerManager; }
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
        private List<SimpleTimer> _timers;
        private List<Character> _characters;

        public CharacterManager(GraphicsDevice graphics)
        {
            _characters = new List<Character>();
            _timers = new List<SimpleTimer>(5);
            _triggerManager = new TriggerManager();

            ScreenWidth = graphics.Viewport.Width;
            ScreenHeight = graphics.Viewport.Height;
        }

        public abstract void Activate(bool instancePreserved);

        public abstract void Deactivate();

        public virtual void Update(GameTime gameTime)
        {
            UpdateCharacters(gameTime);
            UpdateTimers(gameTime);
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

        public virtual void UpdatePlayerInput(GestureSample[] gestures, Weapon currentWeapon, out GestureSample[] remainingGestures)
        {
            remainingGestures = gestures;
        }

        protected abstract void UpdateCharacters(GameTime gameTime);

        private void UpdateTimers(GameTime gameTime)
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
