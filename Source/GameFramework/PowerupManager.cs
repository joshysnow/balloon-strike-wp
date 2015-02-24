using System;
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
        public event PowerupEventHandler PickedUp;

        private Vector2 _freezeVelocity;
        private Vector2 _shellVelocity;
        private Vector2 _missileVelocity;
        private Animation _freezeMoveAnimation;
        private Animation _shellMoveAnimation;
        private Animation _missileMoveAnimation;
        private Animation _popAnimation;
        private SoundEffect _popSoundEffect;
        private Random _randomPosition;

        public PowerupManager(GraphicsDevice graphics) : base(graphics) { }

        public override void Initialize()
        {
            _randomPosition = new Random(DateTime.Now.Millisecond);

            Trigger freezeTrigger = new ScoreTrigger(60);
            freezeTrigger.Triggered += FreezeTriggerHandler;
            AddTrigger(freezeTrigger);

            Trigger shellTrigger = new TimeTrigger(TimeSpan.FromSeconds(45));
            shellTrigger.Triggered += ShellTriggerHandler;
            AddTrigger(shellTrigger);

            Trigger missileTrigger = new ScoreTrigger(90);
            missileTrigger.Triggered += MissileTriggerHandler;
            AddTrigger(missileTrigger);

            _freezeVelocity = new Vector2(0, 4.2f);
            _shellVelocity = new Vector2(0, 6f);
            _missileVelocity = new Vector2(0, 7f);

            ResourceManager manager = ResourceManager.Resources;

            _freezeMoveAnimation = manager.GetAnimation("freezemove");
            _shellMoveAnimation = manager.GetAnimation("shellmove");
            _missileMoveAnimation = manager.GetAnimation("missilemove");
            _popAnimation = manager.GetAnimation("popmove");
            _popSoundEffect = manager.GetSoundEffect("pop");
        }

        public override void Activate(bool instancePreserved)
        {
            
        }

        public override void Deactivate()
        {
            
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
                    case PowerupState.Descending:
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

        private void FreezeTriggerHandler(Trigger trigger)
        {
            SimpleTimer freezeTimer = new SimpleTimer(20000);
            freezeTimer.Elapsed += FreezeTimerElapsed;
            Timers.Add(freezeTimer);
        }

        private void ShellTriggerHandler(Trigger trigger)
        {
            SimpleTimer shellTimer = new SimpleTimer(5000);
            shellTimer.Elapsed += ShellTimerElapsed;
            Timers.Add(shellTimer);
        }

        private void MissileTriggerHandler(Trigger trigger)
        {
            SimpleTimer missileTimer = new SimpleTimer(10000);
            missileTimer.Elapsed += MissileTimerElapsed;
            Timers.Add(missileTimer);
        }

        private void FreezeTimerElapsed(SimpleTimer timer)
        {
            SpawnPowerup(PowerupType.Freeze);
        }

        private void ShellTimerElapsed(SimpleTimer timer)
        {
            SpawnPowerup(PowerupType.Shell);
        }

        private void MissileTimerElapsed(SimpleTimer timer)
        {
            SpawnPowerup(PowerupType.Rocket);
        }

        private void SpawnPowerup(PowerupType type)
        {
            Powerup spawn = new Powerup(type);
            Animation moveAnimation = GetAnimation(type);

            int x = _randomPosition.Next(ScreenWidth - (int)(moveAnimation.FrameWidth * moveAnimation.Scale));
            int y = (0 - (int)(moveAnimation.FrameHeight * moveAnimation.Scale));
            Vector2 upperLeft = new Vector2(x, y);

            spawn.Initialize(moveAnimation, _popAnimation, _popSoundEffect, upperLeft, _freezeVelocity, (short)ScreenHeight);
            Characters.Add(spawn);
        }

        private Animation GetAnimation(PowerupType type)
        {
            switch (type)
            {
                case PowerupType.Shell:
                    return _shellMoveAnimation;
                case PowerupType.Rocket:
                    return _missileMoveAnimation;
                case PowerupType.Freeze:
                case PowerupType.Nuke:
                default:
                    return _freezeMoveAnimation;
            }
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
