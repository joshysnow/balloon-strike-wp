using System;
using Microsoft.Xna.Framework.Graphics;
using GameCore;
using GameCore.Physics.Shapes;

namespace GameFramework
{
    public class WeaponFactory
    {
        private static TimeSpan FADE_TIME = TimeSpan.FromSeconds(1.5);

        private WeaponModel _tapWeaponModel;
        private WeaponModel _shotgunWeaponModel;
        private WeaponModel _bazookaWeaponModel;

        public WeaponFactory() { }

        public void Initialize() 
        {
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
                case WeaponType.Tap:
                default:
                    newWeapon = MakeDefault();
                    break;
            }

            return newWeapon;
        }

        private void InitializeWeaponModels()
        {
            const byte tapDamage = 1;
            const byte shotgunDamage = 2;
            const byte bazookaDamage = 3;

            Texture2D tXHairTexture = ResourceManager.Resources.GetTexture("xhair_tap");
            Texture2D sXHairTexture = ResourceManager.Resources.GetTexture("xhair_shotgun");
            Texture2D bXHairTexture = ResourceManager.Resources.GetTexture("xhair_bazooka");

            _tapWeaponModel = new WeaponModel(WeaponType.Tap, tapDamage, tXHairTexture);
            _shotgunWeaponModel = new WeaponModel(WeaponType.Shotgun, shotgunDamage, sXHairTexture);
            _bazookaWeaponModel = new WeaponModel(WeaponType.Bazooka, bazookaDamage, bXHairTexture);
        }

        private Weapon MakeDefault()
        {
            Weapon tap = new Weapon(_tapWeaponModel, ref FADE_TIME);

            return tap;
        }

        private Weapon MakeShotgun()
        {
            Weapon shotgun = new Weapon(_shotgunWeaponModel, ref FADE_TIME);
            shotgun.Ammo = 6;

            return shotgun;
        }

        private Weapon MakeBazooka()
        {
            Weapon bazooka = new Weapon(_bazookaWeaponModel, ref FADE_TIME);
            bazooka.Ammo = 2;

            return bazooka;
        }
    }
}