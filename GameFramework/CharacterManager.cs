using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;

namespace GameFramework
{
    public abstract class CharacterManager : Serializable
    {
        protected List<Character> Characters
        {
            get { return _characters; }
        }

        protected List<Spawner> Spawners
        {
            get { return _spawners; }
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
        
        private List<Character> _characters;
        private List<Spawner> _spawners;

        public CharacterManager(GraphicsDevice graphics)
        {
            _characters = new List<Character>();
            _spawners = new List<Spawner>(5);

            ScreenWidth = graphics.Viewport.Width;
            ScreenHeight = graphics.Viewport.Height;
        }

        public abstract void Activate(bool instancePreserved);

        public abstract void Deactivate();

        public virtual void Update(GameTime gameTime)
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

        public virtual void UpdatePlayerInput(GestureSample[] gestures, Weapon currentWeapon, out GestureSample[] remainingGestures)
        {
            remainingGestures = gestures;
        }

        protected abstract void UpdateCharacters(GameTime gameTime);

        protected XElement DehydrateSpawners()
        {
            XElement spawnersRoot = new XElement("Spawners");
            XElement spawnerNode;

            foreach (Spawner spawner in _spawners)
            {
                spawnerNode = spawner.Dehydrate();
                spawnersRoot.Add(spawnerNode);
            }

            return spawnersRoot;
        }

        private void UpdateSpawners(GameTime gameTime)
        {
            foreach (Spawner spawner in _spawners)
            {
                spawner.Update(gameTime);
            }
        }
    }
}
