using System;

namespace GameFramework
{
    public class WeaponModel
    {
        public WeaponType Type
        {
            get { return _type; }
        }

        public byte Damage
        {
            get { return _damage; }
        }

        private WeaponType _type;
        private byte _damage;

        public WeaponModel(WeaponType type, byte damage)
        {
            _type = type;
            _damage = damage;
        }
    }
}
