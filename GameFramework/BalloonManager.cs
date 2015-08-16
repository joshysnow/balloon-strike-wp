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
        private Vector2 _redVelocity;
        private Vector2 _greenVelocity;
        private Vector2 _blueVelocity;
        private Animation _greenMoveAnimation;
        private Animation _redMoveAnimation;
        private Animation _blueMoveAnimation;
        private Animation _popAnimation;
        private Animation _hitAnimation;
        private SoundEffect _popSoundEffect;
        private BalloonManagerState _managerState;
        private SimpleTimer _freezeTimer;
        private Random _randomPosition;

        public BalloonManager(GraphicsDevice graphics) : base(graphics) { }

        public override void Initialize()
        {
            _pool = new BalloonPool(50);
            _pool.Fill();

            _managerState = BalloonManagerState.Normal;
            _randomPosition = new Random(DateTime.Now.Millisecond);

            // Add two triggers for when to begin spawning the other balloons.
            Trigger blueSpawnStart = new TimeTrigger(TimeSpan.FromSeconds(20));
            blueSpawnStart.Triggered += BlueSpawnStartTriggerHandler;
            Triggers.AddTrigger(blueSpawnStart);

            Trigger redSpawnStart = new TimeTrigger(TimeSpan.FromSeconds(45));
            redSpawnStart.Triggered += RedSpawnStartTriggerHandler;
            Triggers.AddTrigger(redSpawnStart);

            Trigger velocityChange = new TimeTrigger(TimeSpan.FromSeconds(180)); // 3 minutes
            velocityChange.Triggered += VelocityChangeTriggerHandler;
            Triggers.AddTrigger(velocityChange);

            // Start green balloon immediately.
            VariableTimer _greenTimer = new VariableTimer(4000, 0.9f, 750);
            _greenTimer.Elapsed += GreenTimerElapsed;
            Timers.Add(_greenTimer);

#warning EXPERIMENT START
            TimeTrigger massAttackTimer = new TimeTrigger(TimeSpan.FromSeconds(30));
            massAttackTimer.Triggered += MassAttackTimerTriggered;
            Triggers.AddTrigger(massAttackTimer);
#warning EXPERIMENT END

            _redVelocity = new Vector2(0, -8f);
            _greenVelocity = new Vector2(0, -5f);
            _blueVelocity = new Vector2(0, -6.5f);

            ResourceManager manager = ResourceManager.Resources;

            _greenMoveAnimation = manager.GetAnimation("greenmove");
            _redMoveAnimation = manager.GetAnimation("redmove");
            _blueMoveAnimation = manager.GetAnimation("bluemove");

            _popAnimation = manager.GetAnimation("popmove");
            _hitAnimation = manager.GetAnimation("hitmove");

            _popSoundEffect = manager.GetSoundEffect("pop");
        }

        public override void Activate(bool instancePreserved)
        {
            
        }

        public override void Deactivate()
        {
            // TODO: Store triggers, timers and balloon
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

        private void BlueSpawnStartTriggerHandler(Trigger trigger)
        {
            VariableTimer blueTimer = new VariableTimer(7500, 0.9f, 1000, true);
            blueTimer.Elapsed += BlueTimerElapsed;
            Timers.Add(blueTimer);
        }

        private void RedSpawnStartTriggerHandler(Trigger trigger)
        {
            VariableTimer redTimer = new VariableTimer(7500, 0.9f, 1000, true);
            redTimer.Elapsed += RedTimerElapsed;
            Timers.Add(redTimer);
        }

        private void VelocityChangeTriggerHandler(Trigger trigger)
        {
            _greenVelocity.Y = -9f;
            _blueVelocity.Y = -11f;
            _redVelocity.Y = -13f;
        }

        private void MassAttackTimerTriggered(Trigger trigger)
        {
            SimpleTimer attackTimer = new SimpleTimer(TimeSpan.FromSeconds(30).Milliseconds);
            attackTimer.Elapsed += AttackTimerElapsed;
            Timers.Add(attackTimer);
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

        private void AttackTimerElapsed(SimpleTimer timer)
        {
            for (int i = 0; i < 3; i++)
            {
                SpawnBalloon(BalloonColor.Red);
            }
        }

        private void SpawnBalloon(BalloonColor colour)
        {
            Balloon spawn;

            if (_pool.Size() > 0)
            {
                spawn = _pool.Pop();
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
            spawn.Initialize(moveAnimation, _hitAnimation, _popAnimation, _popSoundEffect, new Vector2(x, ScreenHeight), velocity, health);
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
