using System;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameCore.Physics.Shapes;

namespace GameFramework
{
    public class WeaponFactory
    {
        private static TimeSpan FADE_TIME = TimeSpan.FromSeconds(1.5);

        private CrosshairModel _fingerXHairModel;
        private CrosshairModel _shotgunXHairModel;
        private CrosshairModel _bazookaXHairModel;

        private WeaponModel _fingerWeaponModel;
        private WeaponModel _shotgunWeaponModel;
        private WeaponModel _bazookaWeaponModel;

        public WeaponFactory() { }

        public void Initialize() 
        {
            InitializeCrossHairModels();
            InitializeWeaponModels();
        }

        public Weapon MakeWeapon(WeaponType type)
        {
            Weapon newWeapon = null;

            switch (type)
            {
                case WeaponType.Shotgun:
                    newWeapon = MakeShotgun();
                    break;
                case WeaponType.Bazooka:
                    newWeapon = MakeBazooka();
                    break;
                case WeaponType.Finger:
                default:
                    newWeapon = MakeDefault();
                    break;
            }

            return newWeapon;
        }

        private void InitializeCrossHairModels()
        {
            Texture2D fXHairTexture = ResourceManager.Resources.GetTexture("xhair_finger");
            Texture2D sXHairTexture = ResourceManager.Resources.GetTexture("xhair_shotgun");
            Texture2D bXHairTexture = ResourceManager.Resources.GetTexture("xhair_bazooka");

            Circle circle = new Circle(); 
            circle.Radius = (fXHairTexture.Width / 2);

            _fingerXHairModel = new CrosshairModel(circle, fXHairTexture);

            circle = new Circle();
            circle.Radius = (sXHairTexture.Width / 2);

            _shotgunXHairModel = new CrosshairModel(circle, sXHairTexture);

            circle = new Circle();
            circle.Radius = (bXHairTexture.Width / 2);

            _bazookaXHairModel = new CrosshairModel(circle, bXHairTexture);
        }

        private void InitializeWeaponModels()
        {
            const byte fingerDamage = 1;
            const byte shotgunDamage = 2;
            const byte bazookaDamage = 3;

            _fingerWeaponModel = new WeaponModel(WeaponType.Finger, fingerDamage);
            _shotgunWeaponModel = new WeaponModel(WeaponType.Shotgun, shotgunDamage);
            _bazookaWeaponModel = new WeaponModel(WeaponType.Bazooka, bazookaDamage);
        }

        private Weapon MakeDefault()
        {
            Weapon finger = new Weapon(_fingerWeaponModel, new Crosshair(_fingerXHairModel, ref FADE_TIME));

            return finger;
        }

        private Weapon MakeShotgun()
        {
            Weapon shotgun = new Weapon(_shotgunWeaponModel, new Crosshair(_shotgunXHairModel, ref FADE_TIME));

            return shotgun;
        }

        private Weapon MakeBazooka()
        {
            Weapon bazooka = new Weapon(_bazookaWeaponModel, new Crosshair(_bazookaXHairModel, ref FADE_TIME));

            return bazooka;
        }
    }
}