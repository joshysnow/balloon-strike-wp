using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;

namespace sandbox8
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
        private SimpleTimer _greenTimer;
        private SimpleTimer _freezeTimer;
        private SimpleTimer _managerFreezeTimer;
        private VariableSpawnTimer _blueTimer;
        private VariableSpawnTimer _redTimer;
        private Vector2 _redVelocity;
        private Vector2 _greenVelocity;
        private Vector2 _blueVelocity;
        private Animation _greenMoveAnimation;
        private Animation _redMoveAnimation;
        private Animation _blueMoveAnimation;
        private Animation _popAnimation;
        private SoundEffect _popSoundEffect;
        private BalloonManagerState _managerState;

        private const short _screenHeight = 800;
        private const short _screenWidth = 480;
        private Random _randomPosition;

        public BalloonManager() : base()
        {
            Setup();
        }

        public void Reset()
        {
            Setup();
        }

        public void ApplyPowerup(PowerupType type)
        {
            switch (type)
            {
                case PowerupType.Freeze:
                    {
                        byte index = 0;
                        while (index < _characters.Count)
                        {
                            ((Balloon)_characters[index++]).Freeze(2000);
                        }

                        _managerState = BalloonManagerState.Frozen;
                        _managerFreezeTimer = new SimpleTimer();
                        _managerFreezeTimer.Initialize(2000);
                    }
                    break;
                case PowerupType.Nuke:
                case PowerupType.Shell:
                case PowerupType.Missile:
                default:
                    break;
            }
        }

        public override void UpdatePlayerInput(GestureSample[] gestures, Weapon currentWeapon, out GestureSample[] remainingGestures)
        {
            int index;
            Balloon balloon;
            float radius = currentWeapon.Crosshair.Radius;
            float damage = currentWeapon.Damage;
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
                    balloon = (Balloon)_characters[index];
                    if (balloon.Intersects(gesture.Position, radius))
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

        protected override void UpdateCharacters(GameTime gameTime)
        {
            byte index = 0;
            Balloon balloon;
            while (index < _characters.Count)
            {
                balloon = (Balloon)_characters[index];

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
                            _characters.RemoveAt(index);
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
                if (_greenTimer.Update(gameTime))
                {
                    SpawnBalloon(BalloonColor.Green);
                }

                if (_blueTimer.Update(gameTime))
                {
                    SpawnBalloon(BalloonColor.Blue);
                }

                if (_redTimer.Update(gameTime))
                {
                    SpawnBalloon(BalloonColor.Red);
                }
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

        private void Setup()
        {
            _managerState = BalloonManagerState.Normal;
            _balloonMemory = new LinkedList<Balloon>();
            _randomPosition = new Random(DateTime.Now.Millisecond);

            _greenTimer = new SimpleTimer();
            _greenTimer.Initialize(1500);

            _freezeTimer = new SimpleTimer();
            _freezeTimer.Initialize(15000);

            _blueTimer = new VariableSpawnTimer();
            _blueTimer.Initialize(5000, 0.7f, 1500);

            _redTimer = new VariableSpawnTimer();
            _redTimer.Initialize(15000, 0.8f, 2500);

            _redVelocity = new Vector2(0, -9.2f);
            _greenVelocity = new Vector2(0, -5.1f);
            _blueVelocity = new Vector2(0, -7.15f);

            ResourceManager manager = ResourceManager.Manager;

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

            int x = _randomPosition.Next(_screenWidth - (int)(moveAnimation.AnimationTexture.Width * moveAnimation.Scale));
            spawn.Initialize(moveAnimation, _popAnimation, _popSoundEffect, new Vector2(x, _screenHeight), velocity, health);
            _characters.Add(spawn);
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