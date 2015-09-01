using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using GameCore;

namespace GameFramework
{
    public class PowerupFactory
    {
        private PowerupModel _freezeModel;
        private PowerupModel _shotgunShellModel;
        private PowerupModel _missileModel;

        private int _screenHeight;

        public PowerupFactory() { }

        public void Initialize(int screenHeight)
        {
            _screenHeight = screenHeight;

            ResourceManager manager = ResourceManager.Resources;

            Animation freezeMoveAnimation = manager.GetAnimation("freezemove");
            Animation shellMoveAnimation = manager.GetAnimation("shellmove");
            Animation missileMoveAnimation = manager.GetAnimation("missilemove");
            Animation pickupAnimation = manager.GetAnimation("popmove");
            SoundEffect pickupSoundEffect = manager.GetSoundEffect("pickup_ammo");

            Vector2 freezeVelocity = new Vector2(0, 4.2f);
            Vector2 shellVelocity = new Vector2(0, 6f);
            Vector2 missileVelocity = new Vector2(0, 7f);

            _freezeModel = new PowerupModel(freezeMoveAnimation, pickupAnimation, pickupSoundEffect, ref freezeVelocity);
            _shotgunShellModel = new PowerupModel(shellMoveAnimation, pickupAnimation, pickupSoundEffect, ref shellVelocity);
            _missileModel = new PowerupModel(missileMoveAnimation, pickupAnimation, pickupSoundEffect, ref missileVelocity);
        }

        public Powerup MakePowerup(PowerupType type, ref Vector2 position)
        {
            Powerup make = new Powerup(type);

            switch (type)
            {
                case PowerupType.Freeze:
                    make.Initialize(_freezeModel, ref position, _screenHeight);
                    break;
                case PowerupType.Rocket:
                    make.Initialize(_missileModel, ref position, _screenHeight);
                    break;
                case PowerupType.Shell:
                default:
                    make.Initialize(_shotgunShellModel, ref position, _screenHeight);
                    break;
            }

            return make;
        }
    }
}
