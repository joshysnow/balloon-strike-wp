using System;
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

        private BalloonPool _pool;
        private BalloonFactory _factory;
        private BalloonManagerState _managerState;
        private SimpleTimer _freezeTimer;
        private Random _randomPosition;

        public BalloonManager(GraphicsDevice graphics) : base(graphics) { }

        public override void Initialize()
        {
            _pool = new BalloonPool(50);
            _pool.Fill();

            _factory = new BalloonFactory();
            _factory.Initialize();

            _managerState = BalloonManagerState.Normal;
            _randomPosition = new Random(DateTime.Now.Millisecond);

            // TODO: Use new spawner wrapper
            //  - Get pool balloon
            //  - Put timer and pool balloon into spawner
            //  - Hook spawner to spawn function
            //
            // Note: Pointing all spawner objects to the same function won't
            // matter as this is not a multi-threaded environment so values
            // won't be changed by many spawners (single thread scenario).

            // Create a timer for this spawner
            VariableTimer greenTimer = new VariableTimer(4000, 0.9f, 2000); // was 750
            CreateSpawner(BalloonColor.Green, greenTimer);

            // Add two triggers for when to begin spawning the other balloons.
//            Trigger blueSpawnStart = new TimeTrigger(TimeSpan.FromSeconds(20));
//            blueSpawnStart.Triggered += BlueSpawnStartTriggerHandler;
//            Triggers.AddTrigger(blueSpawnStart);

//            Trigger redSpawnStart = new TimeTrigger(TimeSpan.FromSeconds(45));
//            redSpawnStart.Triggered += RedSpawnStartTriggerHandler;
//            Triggers.AddTrigger(redSpawnStart);

//            Trigger velocityChange = new TimeTrigger(TimeSpan.FromSeconds(180)); // 3 minutes
//            velocityChange.Triggered += VelocityChangeTriggerHandler;
//            Triggers.AddTrigger(velocityChange);

//            // EXPERIMENT
//            TimeTrigger massAttackTimer = new TimeTrigger(TimeSpan.FromSeconds(30));
//            massAttackTimer.Triggered += MassAttackTimerTriggered;
//            Triggers.AddTrigger(massAttackTimer);
        }

        public override void Activate(bool instancePreserved)
        {
            // TODO
        }

        public override void Deactivate()
        {
            // TODO
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

        public override void Update(GameTime gameTime)
        {
            if (_managerState == BalloonManagerState.Frozen)
            {
                UpdateFrozenState(gameTime);
            }
            else
            {
                base.Update(gameTime);
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

        private void CreateSpawner(BalloonColor color, SimpleTimer timer)
        {
            // Get a balloon to use as prototype.
            Balloon prototype = _pool.Pop();

            // Set color for type of balloon we want to spawn.
            prototype.Color = color;

            // Make the prototype.
            Vector2 position = Vector2.Zero;
            _factory.MakeBalloon(color, ref position, ref prototype);

            // Instantiate spawner.
            Spawner spawner = new Spawner(timer, prototype);

            // Listen for when to spawn.
            spawner.Spawn += SpawnerSpawnHandler;

            // TODO: Manager this collection! Do all managers use timers? For this game yes but is it coupled?
            // Want to hold spawners not timers.
            // Spawners to hold info on starting (like trigger).
            Timers.Add(timer);
        }

        private void SpawnerSpawnHandler(Spawner sender, Character prototype)
        {
            // To spawn balloon
            //  - Get balloon color
            //  - Set position
            //  - Get balloon node
            //  - Use factory to make balloon
            //  - Add new balloon to manager

            // Convert prototype into type of character.
            Balloon proto = (Balloon)prototype;

            // Get type of balloon to make.
            BalloonColor spawnColor = proto.Color;

            // Create a random starting coordinate.
            int max = ScreenWidth - (int)proto.Size.X;  // Make sure the balloon cannot be spawned off screen.
            int x = _randomPosition.Next(max);

            // Create starting coordinate.
            Vector2 position = new Vector2(x, ScreenHeight);

            // Get preallocated resource.
            Balloon make = _pool.Pop();

            // Use factory to make balloon. Check we have resource first otherwise make it.
            if (make != null)
                _factory.MakeBalloon(spawnColor, ref position, ref make);
            else
                _factory.MakeBalloon(spawnColor, ref position);

            Characters.Add(make);
        }

        //private void BlueSpawnStartTriggerHandler(Trigger trigger)
        //{
        //    VariableTimer blueTimer = new VariableTimer(7500, 0.9f, 1000, true);
        //    blueTimer.Elapsed += BlueTimerElapsed;
        //    Timers.Add(blueTimer);
        //}

        //private void RedSpawnStartTriggerHandler(Trigger trigger)
        //{
        //    VariableTimer redTimer = new VariableTimer(7500, 0.9f, 1000, true);
        //    redTimer.Elapsed += RedTimerElapsed;
        //    Timers.Add(redTimer);
        //}

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
