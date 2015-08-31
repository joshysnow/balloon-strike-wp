using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using GameCore;
using GameCore.Memory;
using GameCore.Timers;
using GameCore.Triggers;
using GameFramework.Triggers;

namespace GameFramework
{
    public delegate void BalloonEventHandler(Balloon balloon);

    public class BalloonManager : CharacterManager
    {
        private enum BalloonManagerState : byte
        {
            Normal = 0x01,
            Frozen = 0x02
        }

        public event BalloonEventHandler Popped;
        public event BalloonEventHandler Escaped;

        private const string STORAGE_FILE_NAME = "BALLOON_MANAGER.xml";

        private List<Spawner> _spawners;
        private BalloonPool _pool;
        private BalloonFactory _factory;
        private BalloonManagerState _managerState;
        private SimpleTimer _freezeTimer;
        private Random _randomPosition;

        public BalloonManager(GraphicsDevice graphics) : base(graphics) { }

        public override void Activate(bool instancePreserved)
        {
            if (instancePreserved)
            {
                // Nothing to do here.
            }
            else
            {
                // Setup this manager.
                Initialize();

                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (storage.FileExists(STORAGE_FILE_NAME))
                    {
                        using (IsolatedStorageFileStream stream = storage.OpenFile(STORAGE_FILE_NAME, FileMode.Open))
                        {
                            XDocument doc = XDocument.Load(stream);
                            XElement root = doc.Root;

                            // Rehydrate manager state.
                            BalloonManagerState temp = BalloonManagerState.Normal;
                            _managerState = (BalloonManagerState)Enum.Parse(temp.GetType(), root.Attribute("State").Value, false);

                            // Rehydrate freeze timer (if set).
                            if (_managerState == BalloonManagerState.Frozen)
                            {
                                XElement xTimer = root.Element("Timer");

                                if (xTimer != null)
                                {
                                    _freezeTimer = SimpleTimer.Rehydrate(xTimer);
                                }
                            }

                            // Rehydrate balloons.
                            XElement xBalloons = root.Element("Balloons");
                            RehydrateBalloons(xBalloons);
                            
                            // Rehydrate spawners.
                            XElement xSpawners = root.Element("Spawners");
                            RehydrateSpawners(xSpawners);                            
                        }
                        
                        storage.DeleteFile(STORAGE_FILE_NAME);
                    }
                    else
                    {
                        // Nothing to rehydrate from so initialize as new.
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
                XElement root = new XElement("BalloonManager", 
                    new XAttribute("State", _managerState)
                    );

                // Dehydrate frozen timer.
                if ((_managerState == BalloonManagerState.Frozen) && (_freezeTimer != null))
                {
                    root.Add(_freezeTimer.Dehydrate());
                }

                // Dehydrate balloons.
                XElement balloonsRoot = new XElement("Balloons");
                XElement balloonNode;

                foreach (Character character in Characters)
                {
                    balloonNode = ((Balloon)character).Dehydrate();
                    balloonsRoot.Add(balloonNode);
                }

                // Dehydrate spawners.
                XElement spawnersRoot = new XElement("Spawners");
                XElement spawnerNode;

                foreach (Spawner spawner in _spawners)
                {
                    spawnerNode = spawner.Dehydrate();
                    spawnersRoot.Add(spawnerNode);
                }

                root.Add(balloonsRoot);
                root.Add(spawnersRoot);
                doc.Add(root);

                using (IsolatedStorageFileStream stream = storage.CreateFile(""))
                {
                    doc.Save(stream);
                }
            }
        }

        public override void UpdatePlayerInput(GestureSample[] gestures, Weapon currentWeapon, out GestureSample[] remainingGestures)
        {
            int index;
            Balloon balloon;
            float damage = currentWeapon.Damage;
            GameCore.Physics.Shapes.Circle circle = currentWeapon.Circle;
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
                    balloon = (Balloon)Characters[index];

                    // Check to ensure that the same balloon isn't used if there are more than one gestures).
                    if ((balloon.State == BalloonState.Alive || balloon.State == BalloonState.Frozen || balloon.State == BalloonState.Hit) && balloon.Intersects(circle))
                    {
                        balloon.Attack(damage);

                        if (weaponType == WeaponType.Tap)
                        {
                            temp.Remove(gesture);
                            break;
                        }
                    }
                    index--;
                }
            }

            remainingGestures = temp.ToArray();
        }

        public override void Update(GameTime gameTime)
        {
            if (_managerState == BalloonManagerState.Frozen)
            {
                UpdateFrozenState(gameTime);
            }
            else
            {
                base.Update(gameTime);
                UpdateSpawners(gameTime);
            }
        }

        public void ApplyPowerup(PowerupType type)
        {
            switch (type)
            {
                case PowerupType.Freeze:
                    {
                        byte index = 0;
                        while (index < Characters.Count)
                        {
                            ((Balloon)Characters[index++]).Freeze(2000);
                        }

                        _managerState = BalloonManagerState.Frozen;
                        _freezeTimer = new SimpleTimer(2000);
                    }
                    break;
                case PowerupType.Nuke:
                default:
                    break;
            }
        }

        protected override void UpdateCharacters(GameTime gameTime)
        {
            byte index = 0;
            Balloon balloon;
            while (index < Characters.Count)
            {
                balloon = (Balloon)Characters[index];

                switch (balloon.State)
                {
                    case BalloonState.Alive:
                    case BalloonState.Hit:
                    case BalloonState.Frozen:
                    case BalloonState.Dying:
                        {
                            balloon.Update(gameTime);
                            index++;
                        }
                        break;
                    case BalloonState.Popped:
                        {
                            balloon.Update(gameTime);
                            RaisePopped(balloon);
                            index++;
                        }
                        break;
                    case BalloonState.Escaped:
                        {
                            balloon.Update(gameTime);
                            RaiseEscaped(balloon);
                            index++;
                        }
                        break;
                    case BalloonState.Dead:
                        {
                            balloon.Uninitialize();
                            _pool.Push(balloon);
                            Characters.RemoveAt(index);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void Initialize()
        {
            _spawners = new List<Spawner>(5);

            _pool = new BalloonPool(50);
            _pool.Fill();

            // TODO: Perhaps pass in the pool to use on initializing the factory to build from.
            _factory = new BalloonFactory();
            _factory.Initialize();

            _managerState = BalloonManagerState.Normal;
            _randomPosition = new Random(DateTime.Now.Millisecond);
        }

        private void InitializeDefault()
        {
            // Use new spawner wrapper
            //  - Get pool balloon
            //  - Put timer and pool balloon into spawner
            //  - Hook spawner to spawn function
            //
            // Note: Pointing all spawner objects to the same function won't
            // matter as this is not a multi-threaded environment so values
            // won't be changed by many spawners (single thread scenario).

            // Create a timer for this spawner
            VariableTimer greenTimer = new VariableTimer(4000, 0.9f, 750);
            Spawner greenSpawner = CreateSpawner(BalloonColor.Green, greenTimer);
            _spawners.Add(greenSpawner);

            VariableTimer blueTimer = new VariableTimer(7500, 0.9f, 1000, true);
            TimeSpan startTime = TimeSpan.FromSeconds(20);
            Spawner blueSpawner = CreateSpawner(BalloonColor.Blue, blueTimer, (float)startTime.TotalMilliseconds);
            _spawners.Add(blueSpawner);

            VariableTimer redTimer = new VariableTimer(7500, 0.9f, 1000, true);
            startTime = TimeSpan.FromSeconds(45);
            Spawner redSpawner = CreateSpawner(BalloonColor.Red, redTimer, (float)startTime.TotalMilliseconds);
            _spawners.Add(redSpawner);

            //            Trigger velocityChange = new TimeTrigger(TimeSpan.FromSeconds(180)); // 3 minutes
            //            velocityChange.Triggered += VelocityChangeTriggerHandler;
            //            Triggers.AddTrigger(velocityChange);

            //            // EXPERIMENT
            //            TimeTrigger massAttackTimer = new TimeTrigger(TimeSpan.FromSeconds(30));
            //            massAttackTimer.Triggered += MassAttackTimerTriggered;
            //            Triggers.AddTrigger(massAttackTimer);
        }

        private void RehydrateBalloons(XElement balloons)
        {
            // Balloon pre-declared members.
            float x, y;
            Vector2 position;
            BalloonColor color;
            BalloonColor tempColor = BalloonColor.Green;

            Balloon balloon;

            // Rehydrate all balloons.
            foreach (XElement xBalloon in balloons.Elements())
            {
                // Rehydrate position.
                x = float.Parse(xBalloon.Attribute("UL-X").Value);
                y = float.Parse(xBalloon.Attribute("UL-Y").Value);
                position = new Vector2(x, y);

                color = (BalloonColor)Enum.Parse(tempColor.GetType(), xBalloon.Attribute("Color").Value, false);

                balloon = CreateBalloon(color, ref position);
                Characters.Add(balloon);
            }
        }

        private void RehydrateSpawners(XElement spawners)
        {
            Spawner spawner;
            Vector2 prototypePosition = Vector2.Zero;

            _spawners = new List<Spawner>(5);

            foreach (XElement xSpawner in spawners.Elements())
            {
                // Note: Remember for spawners, get the Spawns attribute to know what balloon
                // prototype to build for it.

                string spawns = xSpawner.Attribute("Spawns").Value;
                string colorValue = "Green";

                if (spawns.Contains("Balloon"))
                {
                    colorValue = spawns.Split('_').Last();
                }

                // TODO: Perhaps have spawns as an array to hold arguments i.e. balloon color etc as attributes

                BalloonColor color = BalloonColor.Green;
                color = (BalloonColor)Enum.Parse(color.GetType(), colorValue, false);

                Balloon prototype = CreateBalloon(color, ref prototypePosition);
                spawner = Spawner.Rehydrate(xSpawner, prototype);
                _spawners.Add(spawner);
            }
        }

        private void UpdateFrozenState(GameTime gameTime)
        {
            // Still want to update characters.
            UpdateCharacters(gameTime);

            if (_freezeTimer.Update(gameTime))
            {
                _freezeTimer = null;
                _managerState = BalloonManagerState.Normal;
            }
        }

        private void UpdateSpawners(GameTime gameTime)
        {
            foreach (Spawner spawner in _spawners)
            {
                spawner.Update(gameTime);
            }
        }

        private Spawner CreateSpawner(BalloonColor color, VariableTimer timer, float startTime = 0)
        {
            // Make the prototype.
            Vector2 position = Vector2.Zero;
            Balloon prototype = CreateBalloon(color, ref position);

            // Instantiate spawner.
            Spawner spawner = new Spawner(timer, prototype, startTime);

            // Listen for when to spawn.
            spawner.Spawn += SpawnerSpawnHandler;

            return spawner;
        }

        private Balloon CreateBalloon(BalloonColor color, ref Vector2 position)
        {
            // Get a balloon to use as prototype.
            Balloon balloon = _pool.Pop();

            // Check if pool is empty or not.
            if (balloon != null)
                _factory.MakeBalloon(color, ref position, ref balloon);
            else
                _factory.MakeBalloon(color, ref position);

            return balloon;
        }

        private void SpawnerSpawnHandler(Spawner sender, ISpawnable prototype)
        {
            // To spawn balloon
            //  - Create random position
            //  - Get balloon color
            //  - Use factory to make balloon
            //  - Add new balloon to manager

            Balloon proto = (Balloon)prototype;

            // Create a random starting coordinate.
            int max = ScreenWidth - (int)proto.Size.X;  // Make sure the balloon cannot be spawned off screen.
            int x = _randomPosition.Next(max);

            // Create starting coordinate.
            Vector2 position = new Vector2(x, ScreenHeight);

            // Get type of balloon to make.
            BalloonColor spawnColor = proto.Color;

            // Make balloon.
            Balloon balloon = CreateBalloon(spawnColor, ref position);

            Characters.Add(balloon);
        }

        //private void MassAttackTimerTriggered(Trigger trigger)
        //{
        //    SimpleTimer attackTimer = new SimpleTimer(TimeSpan.FromSeconds(30).Milliseconds);
        //    attackTimer.Elapsed += AttackTimerElapsed;
        //    Timers.Add(attackTimer);
        //}

        //private void GreenTimerElapsed(SimpleTimer timer)
        //{
        //    SpawnBalloon(BalloonColor.Green);
        //}

        //private void BlueTimerElapsed(SimpleTimer timer)
        //{
        //    SpawnBalloon(BalloonColor.Blue);
        //}

        //private void RedTimerElapsed(SimpleTimer timer)
        //{
        //    SpawnBalloon(BalloonColor.Red);
        //}

        //private void AttackTimerElapsed(SimpleTimer timer)
        //{
        //    for (int i = 0; i < 3; i++)
        //    {
        //        SpawnBalloon(BalloonColor.Red);
        //    }
        //}

        //private void SpawnBalloon(BalloonColor colour)
        //{
        //    Balloon spawn;

        //    if (_pool.Size() > 0)
        //    {
        //        spawn = _pool.Pop();
        //    }
        //    else
        //    {
        //        spawn = new Balloon();
        //    }
            
        //    spawn.Color = colour;

        //    Vector2 velocity = new Vector2(0, 3.1f);
        //    Animation moveAnimation;
        //    float health;

        //    switch (colour)
        //    {
        //        case BalloonColor.Red:
        //            health = 3;
        //            velocity = _redVelocity;
        //            moveAnimation = _redMoveAnimation;
        //            break;
        //        case BalloonColor.Blue:
        //            health = 2;
        //            moveAnimation = _blueMoveAnimation;
        //            velocity = _blueVelocity;
        //            break;
        //        case BalloonColor.Green:
        //        default:
        //            health = 1;
        //            velocity = _greenVelocity;
        //            moveAnimation = _greenMoveAnimation;
        //            break;
        //    }

        //    int x = _randomPosition.Next(ScreenWidth - (int)(moveAnimation.AnimationTexture.Width * moveAnimation.Scale));
        //    spawn.Initialize(moveAnimation, _hitAnimation, _popAnimation, _popSoundEffect, new Vector2(x, ScreenHeight), velocity, health);
        //    Characters.Add(spawn);
        //}

        private void RaisePopped(Balloon balloon)
        {
            if (Popped != null)
            {
                Popped(balloon);
            }
        }

        private void RaiseEscaped(Balloon balloon)
        {
            if (Escaped != null)
            {
                Escaped(balloon);
            }
        }
    }
}
