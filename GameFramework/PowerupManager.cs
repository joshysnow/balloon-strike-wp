using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;
using GameCore.Timers;

namespace GameFramework
{
    public delegate void PowerupEventHandler(Powerup powerup);

    public class PowerupManager : CharacterManager
    {
        private const string STORAGE_FILE_NAME = "POWERUP_MANAGER.xml";

        public event PowerupEventHandler PickedUp;

        private PowerupFactory _factory;
        private Random _randomPosition;

        public PowerupManager(GraphicsDevice graphics) : base(graphics) { }

        public override void Activate(bool instancePreserved, bool newGame)
        {
            if (!instancePreserved)
            {
                // Setup manager.
                Initialize();

                if (newGame)
                {
                    InitializeDefault();
                }
                else
                {
                    Rehydrate();
                }
            }
        }

        public override void Deactivate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();
                XElement root = new XElement("PowerupManager");

                // Dehydrate powerups.
                XElement powerupsRoot = new XElement("Powerups");
                XElement powerupNode;

                foreach (Character character in Characters)
                {
                    powerupNode = ((Powerup)character).Dehydrate();
                    powerupsRoot.Add(powerupNode);
                }

                // Dehydrate spawners.
                XElement spawnersRoot = DehydrateSpawners();

                root.Add(powerupsRoot);
                root.Add(spawnersRoot);
                doc.Add(root);

                using (IsolatedStorageFileStream stream = storage.CreateFile(STORAGE_FILE_NAME))
                {
                    doc.Save(stream);
                }
            }
        }

        public override void UpdatePlayerInput(GestureSample[] gestures, Weapon currentWeapon, out GestureSample[] remainingGestures)
        {
            int index;
            Powerup powerup;
            WeaponType weaponType = currentWeapon.Type;

            List<GestureSample> temp = new List<GestureSample>(gestures);

            foreach (GestureSample gesture in gestures)
            {
                if (gesture.GestureType != GestureType.Tap)
                {
                    continue;
                }

                index = (Characters.Count - 1);

                while (index >= 0)
                {
                    powerup = (Powerup)Characters[index];
                    if (powerup.Intersects(gesture.Position))
                    {
                        powerup.Pickup();
                        temp.Remove(gesture);
                        break;
                    }
                    index--;
                }
            }

            remainingGestures = temp.ToArray();
        }

        protected override void UpdateCharacters(GameTime gameTime)
        {
            byte index = 0;
            Powerup powerup;
            while (index < Characters.Count)
            {
                powerup = (Powerup)Characters[index];

                switch (powerup.State)
                {
                    case PowerupState.PickingUp:
                    case PowerupState.Falling:
                        {
                            powerup.Update(gameTime);
                            index++;
                        }
                        break;
                    case PowerupState.Dead:
                        {
                            Characters.RemoveAt(index);
                        }
                        break;
                    case PowerupState.Pickedup:
                        {
                            // Apply the effect.
                            powerup.Update(gameTime);
                            RaisePickedUp(powerup);
                            index++;
                        }
                        break;
                    case PowerupState.Missed:
                        {
                            // Inform player?
                            powerup.Update(gameTime);
                            index++;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void Initialize()
        {
            _randomPosition = new Random(DateTime.Now.Millisecond);

            _factory = new PowerupFactory();
            _factory.Initialize(ScreenHeight);
        }

        private void InitializeDefault()
        {
            VariableTimer freezeTimer = new VariableTimer(20000, 0, 0);
            TimeSpan startTime = TimeSpan.FromSeconds(120);
            Spawner freezeSpawner = CreateSpawner(PowerupType.Freeze, freezeTimer, (float)startTime.TotalMilliseconds);
            Spawners.Add(freezeSpawner);

            VariableTimer shellTimer = new VariableTimer(5000, 0, 0);
            startTime = TimeSpan.FromSeconds(45);
            Spawner shellSpawner = CreateSpawner(PowerupType.Shell, shellTimer, (float)startTime.TotalMilliseconds);
            Spawners.Add(shellSpawner);

            VariableTimer missileTimer = new VariableTimer(10000, 0, 0);
            startTime = TimeSpan.FromSeconds(90);
            Spawner missileSpawner = CreateSpawner(PowerupType.Rocket, missileTimer, (float)startTime.TotalMilliseconds);
            Spawners.Add(missileSpawner);
        }

        private void Rehydrate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists(STORAGE_FILE_NAME))
                {
                    using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Open))
                    {
                        XDocument doc = XDocument.Load(stream);
                        XElement root = doc.Root;

                        // Rehydrate powerups.
                        XElement xPowerups = root.Element("Powerups");
                        RehydratePowerups(xPowerups);

                        XElement xSpawners = root.Element("Spawners");
                        RehydrateSpawners(xSpawners);
                    }

                    storage.DeleteFile(STORAGE_FILE_NAME);
                }
                else
                {
                    InitializeDefault();
                }
            }
        }

        private void RehydratePowerups(XElement powerups)
        {
            Powerup powerup;

            foreach (XElement xPowerup in powerups.Elements())
            {
                powerup = _factory.RehydratePowerup(xPowerup);
                Characters.Add(powerup);
            }
        }

        private void RehydrateSpawners(XElement spawners)
        {
            Spawner spawner;
            Vector2 prototypePosition = Vector2.Zero;

            foreach (XElement xSpawner in spawners.Elements())
            {
                // Note: Remember for spawners, get the Spawns attribute to know what powerup
                // prototype to build for it.

                string spawns = xSpawner.Attribute("Spawns").Value;
                string typeValue = "Shell";

                if (spawns.Contains("Powerup"))
                {
                    typeValue = spawns.Split('_').Last();
                }

                PowerupType type = PowerupType.Shell;
                type = (PowerupType)Enum.Parse(type.GetType(), typeValue, false);

                Vector2 position = Vector2.Zero;
                Powerup prototype = _factory.MakePowerup(type, ref position);

                spawner = Spawner.Rehydrate(xSpawner, prototype);
                spawner.Spawn += SpawnerSpawnHandler;

                Spawners.Add(spawner);
            }
        }

        private Spawner CreateSpawner(PowerupType type, VariableTimer timer, float startTime = 0)
        {
            // Make the prototype.
            Vector2 position = Vector2.Zero;
            Powerup prototype = _factory.MakePowerup(type, ref position);

            // Instantiate spawner.
            Spawner spawner = new Spawner(timer, prototype, startTime);

            // Listen for when to spawn.
            spawner.Spawn += SpawnerSpawnHandler;

            return spawner;
        }

        private void SpawnerSpawnHandler(Spawner sender, ISpawnable prototype)
        {
            Powerup proto = (Powerup)prototype;

            int x = _randomPosition.Next(ScreenWidth - (int)proto.Size.X);
            int y = (int)(proto.Size.Y * -1);
            Vector2 position = new Vector2(x, y);

            Powerup newPowerup = _factory.MakePowerup(proto.Type, ref position);
            Characters.Add(newPowerup);
        }

        private void RaisePickedUp(Powerup powerup)
        {
            if (PickedUp != null)
            {
                PickedUp(powerup);
            }
        }
    }
}
