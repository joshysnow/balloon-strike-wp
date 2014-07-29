using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;

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

        private LinkedList<Balloon> _balloonMemory;
        private Vector2 _redVelocity;
        private Vector2 _greenVelocity;
        private Vector2 _blueVelocity;
        private Animation _greenMoveAnimation;
        private Animation _redMoveAnimation;
        private Animation _blueMoveAnimation;
        private Animation _popAnimation;
        private SoundEffect _popSoundEffect;
        private BalloonManagerState _managerState;
        private SimpleTimer _managerFreezeTimer;
        private Random _randomPosition;

        public BalloonManager(GraphicsDevice graphics, TriggerManager triggers) : base(graphics, triggers) { }

        public override void UpdatePlayerInput(GestureSample[] gestures, Weapon currentWeapon, out GestureSample[] remainingGestures)
        {
            int index;
            Balloon balloon;
            float damage = currentWeapon.Damage;
            Physics.Shapes.Circle circle = currentWeapon.Crosshair.Circle;
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
                    if ((balloon.State == BalloonState.Alive) && balloon.Intersects(circle))
                    {
                        balloon.Attack(damage);

                        if (weaponType == WeaponType.Finger)
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
                        _managerFreezeTimer = new SimpleTimer(2000);
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
                            _balloonMemory.AddFirst(balloon);
                            Characters.RemoveAt(index);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void UpdateSpawners(GameTime gameTime)
        {
            if (_managerState == BalloonManagerState.Normal)
            {
                base.UpdateSpawners(gameTime);
            }
            else
            {
                if(_managerFreezeTimer.Update(gameTime))
                {
                    _managerFreezeTimer = null;
                    _managerState = BalloonManagerState.Normal;
                }
            }
        }

        protected override void Initialize()
        {
            _managerState = BalloonManagerState.Normal;
            _balloonMemory = new LinkedList<Balloon>();
            _randomPosition = new Random(DateTime.Now.Millisecond);

            // Add two triggers for when to begin spawning the other balloons.
            Trigger blueSpawnStart = new TimeTrigger(TimeSpan.FromSeconds(15));
            blueSpawnStart.Triggered += BlueSpawnStartTriggerHandler;
            AddTrigger(blueSpawnStart);

            Trigger redSpawnStart = new TimeTrigger(TimeSpan.FromSeconds(30));
            redSpawnStart.Triggered += RedSpawnStartTriggerHandler;
            AddTrigger(redSpawnStart);

            Trigger velocityChange = new TimeTrigger(TimeSpan.FromSeconds(60));
            velocityChange.Triggered += VelocityChangeTriggerHandler;

            // Start green balloon immediately.
            VariableTimer _greenTimer = new VariableTimer(2000, 0.99f, 750);
            _greenTimer.Elapsed += GreenTimerElapsed;
            Timers.Add(_greenTimer);

            _redVelocity = new Vector2(0, -9.2f);
            _greenVelocity = new Vector2(0, -5.1f);
            _blueVelocity = new Vector2(0, -7.15f);

            ResourceManager manager = ResourceManager.Resources;

            _greenMoveAnimation = manager.GetAnimation("greenmove");
            _redMoveAnimation = manager.GetAnimation("redmove");
            _blueMoveAnimation = manager.GetAnimation("bluemove");

            _popAnimation = manager.GetAnimation("popmove");
            _popSoundEffect = manager.GetSoundEffect("pop");

            Balloon newBalloon;
            while (_balloonMemory.Count < 10)
            {
                newBalloon = new Balloon();
                _balloonMemory.AddFirst(newBalloon);
            }
        }

        private void BlueSpawnStartTriggerHandler(Trigger trigger)
        {
            VariableTimer blueTimer = new VariableTimer(5000, 0.95f, 750);
            blueTimer.Elapsed += BlueTimerElapsed;
            Timers.Add(blueTimer);
        }

        private void RedSpawnStartTriggerHandler(Trigger trigger)
        {
            VariableTimer redTimer = new VariableTimer(5000, 0.95f, 750);
            redTimer.Elapsed += RedTimerElapsed;
            Timers.Add(redTimer);
        }

        private void VelocityChangeTriggerHandler(Trigger trigger)
        {
            _greenVelocity.Y = -9f;
            _blueVelocity.Y = -11f;
            _redVelocity.Y = -13f;
#warning TODO: Could call a function to reuse a trigger to perhaps add more timers? Prevents it from being deleted after this function ends.
        }

        private void GreenTimerElapsed(SimpleTimer timer)
        {
            SpawnBalloon(BalloonColor.Green);
        }

        private void BlueTimerElapsed(SimpleTimer timer)
        {
            SpawnBalloon(BalloonColor.Blue);
        }

        private void RedTimerElapsed(SimpleTimer timer)
        {
            SpawnBalloon(BalloonColor.Red);
        }

        private void SpawnBalloon(BalloonColor colour)
        {
            Balloon spawn;

            if (_balloonMemory.Count > 0)
            {
                spawn = _balloonMemory.First.Value;
                _balloonMemory.RemoveFirst();
            }
            else
            {
                spawn = new Balloon();
            }
            
            spawn.Color = colour;

            Vector2 velocity = new Vector2(0, 3.1f);
            Animation moveAnimation;
            float health;

            switch (colour)
            {
                case BalloonColor.Red:
                    health = 3;
                    velocity = _redVelocity;
                    moveAnimation = _redMoveAnimation;
                    break;
                case BalloonColor.Blue:
                    health = 2;
                    moveAnimation = _blueMoveAnimation;
                    velocity = _blueVelocity;
                    break;
                case BalloonColor.Green:
                default:
                    health = 1;
                    velocity = _greenVelocity;
                    moveAnimation = _greenMoveAnimation;
                    break;
            }

            int x = _randomPosition.Next(ScreenWidth - (int)(moveAnimation.AnimationTexture.Width * moveAnimation.Scale));
            spawn.Initialize(moveAnimation, _popAnimation, _popSoundEffect, new Vector2(x, ScreenHeight), velocity, health);
            Characters.Add(spawn);
        }

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
