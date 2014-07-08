using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace sandbox8
{
    public delegate void PowerupEventHandler(Powerup powerup);

    public class PowerupManager : CharacterManager
    {
        public event PowerupEventHandler PickedUp;

        private SimpleTimer _freezeTimer;
        private SimpleTimer _shellTimer;
        private SimpleTimer _missileTimer;
        private Vector2 _freezeVelocity;
        private Vector2 _shellVelocity;
        private Vector2 _missileVelocity;
        private Animation _freezeMoveAnimation;
        private Animation _shellMoveAnimation;
        private Animation _missileMoveAnimation;
        private Animation _popAnimation;
        private SoundEffect _popSoundEffect;

        private const short _screenHeight = 800;
        private const short _screenWidth = 480;
        private Random _randomPosition;

        public PowerupManager()
        {
            Setup();
        }

        public void Reset()
        {
            Setup();
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

                index = (_characters.Count - 1);

                while (index >= 0)
                {
                    powerup = (Powerup)_characters[index];
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
            while (index < _characters.Count)
            {
                powerup = (Powerup)_characters[index];

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
                            _characters.RemoveAt(index);
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

        protected override void UpdateSpawners(GameTime gameTime)
        {
            if (_freezeTimer.Update(gameTime))
            {
                SpawnPowerup(PowerupType.Freeze);
            }

            if (_shellTimer.Update(gameTime))
            {
                SpawnPowerup(PowerupType.Shell);
            }

            if (_missileTimer.Update(gameTime))
            {
                SpawnPowerup(PowerupType.Missile);
            }
        }


        private void Setup()
        {
            _randomPosition = new Random(DateTime.Now.Millisecond);

            _freezeTimer = new SimpleTimer();
            _freezeTimer.Initialize(5000);
            _freezeVelocity = new Vector2(0, 4.2f);

            _shellTimer = new SimpleTimer();
            _shellTimer.Initialize(5000);
            _shellVelocity = new Vector2(0, 6f);

            _missileTimer = new SimpleTimer();
            _missileTimer.Initialize(7500);
            _missileVelocity = new Vector2(0, 7f);

            ResourceManager manager = ResourceManager.Manager;

            _freezeMoveAnimation = manager.GetAnimation("freezemove");
            _shellMoveAnimation = manager.GetAnimation("shellmove");
            _missileMoveAnimation = manager.GetAnimation("missilemove");
            _popAnimation = manager.GetAnimation("popmove");
            _popSoundEffect = manager.GetSoundEffect("pop");
        }

        private void SpawnPowerup(PowerupType type)
        {
            Powerup spawn = new Powerup(type);
            Animation moveAnimation = GetAnimation(type);

            int x = _randomPosition.Next(_screenWidth - (int)(moveAnimation.FrameWidth * moveAnimation.Scale));
            int y = (0 - (int)(moveAnimation.FrameHeight * moveAnimation.Scale));
            Vector2 upperLeft = new Vector2(x, y);

            spawn.Initialize(moveAnimation, _popAnimation, _popSoundEffect, upperLeft, _freezeVelocity, _screenHeight);
            _characters.Add(spawn);
        }

        private Animation GetAnimation(PowerupType type)
        {
            switch (type)
            {
                case PowerupType.Shell:
                    return _shellMoveAnimation;
                case PowerupType.Missile:
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
