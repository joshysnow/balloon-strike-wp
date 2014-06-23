using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;

namespace sandbox7
{
    public delegate void PowerupEventHandler(Powerup powerup);

    public class PowerupManager
    {
        public event PowerupEventHandler PickedUp;

        private List<Powerup> _powerups;
        private SimpleTimer _freezeTimer;
        private Vector2 _freezeVelocity;
        private Animation _freezeMoveAnimation;
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

        public void UpdatePlayerInput(GestureSample[] gestures)
        {
            int index;

            foreach (GestureSample gesture in gestures)
            {
                if (gesture.GestureType != GestureType.Tap)
                {
                    continue;
                }

                index = (_powerups.Count - 1);

                while (index >= 0)
                {
                    if (_powerups[index].Intersects(gesture.Position))
                    {
                        _powerups[index].Pickup();
                        break;
                    }
                    index--;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            UpdatePowerups(gameTime);
            UpdateSpawners(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            byte index = 0;
            while (index < _powerups.Count)
            {
                _powerups[index++].Draw(spriteBatch);
            }
        }

        private void Setup()
        {
            _powerups = new List<Powerup>();
            _randomPosition = new Random(DateTime.Now.Millisecond);

            _freezeTimer = new SimpleTimer();
            _freezeTimer.Initialize(15000);
            _freezeVelocity = new Vector2(0, 4.2f);

            ResourceManager manager = ResourceManager.Manager;

            _freezeMoveAnimation = manager.GetAnimation("freezemove");
            _popAnimation = manager.GetAnimation("popmove");
            _popSoundEffect = manager.GetSoundEffect("pop");
        }

        private void UpdatePowerups(GameTime gameTime)
        {
            byte index = 0;
            Powerup powerup;
            while (index < _powerups.Count)
            {
                powerup = _powerups[index];

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
                            _powerups.RemoveAt(index);
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

                index++;
            }
        }

        private void UpdateSpawners(GameTime gameTime)
        {
            if (_freezeTimer.Update(gameTime))
            {
                SpawnPowerup(PowerupType.Freeze);
            }
        }

        private void SpawnPowerup(PowerupType type)
        {
            Powerup spawn = new Powerup();
            spawn.Type = type;

            int x = _randomPosition.Next(_screenWidth - (int)(_freezeMoveAnimation.FrameWidth * _freezeMoveAnimation.Scale));
            int y = (0 - (int)(_freezeMoveAnimation.FrameHeight * _freezeMoveAnimation.Scale));
            Vector2 upperLeft = new Vector2(x, y);

            spawn.Initialize(_freezeMoveAnimation, _popAnimation, _popSoundEffect, upperLeft, _freezeVelocity, _screenHeight);
            _powerups.Add(spawn);
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
