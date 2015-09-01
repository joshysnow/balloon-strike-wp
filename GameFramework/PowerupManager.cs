using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;
using GameCore.Timers;
using GameCore.Triggers;
using GameFramework.Triggers;

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

        public override void Activate(bool instancePreserved)
        {
            if (instancePreserved)
            {
                // Everything still in memory, all good!
            }
            else
            {
                // Setup manager.
                Initialize();

                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(STORAGE_FILE_NAME))
                    {
                        using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Open))
                        {
                            XDocument doc = XDocument.Load(stream);

                            // TODO: Rehydrate powerups
                        }
                    }
                    else
                    {
                        InitializeDefault();
                    }
                }

            }
        }

        public override void Deactivate()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                XDocument doc = new XDocument();

                // TODO: Dehydrate powerups

                using (IsolatedStorageFileStream stream = storage.CreateFile(STORAGE_FILE_NAME))
                {
                    //doc.Save(stream);
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
