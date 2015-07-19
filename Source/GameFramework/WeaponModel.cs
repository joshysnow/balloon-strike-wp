using Microsoft.Xna.Framework.Graphics;
using GameCore.Physics.Shapes;

namespace GameFramework
{
    public class WeaponModel
    {
        public Texture2D CrosshairTexture
        {
            get { return _crosshairTexture; }
        }

        public WeaponType Type
        {
            get { return _type; }
        }

        public byte Damage
        {
            get { return _damage; }
        }

        private Texture2D _crosshairTexture;
        private WeaponType _type;
        private byte _damage;

        public WeaponModel(WeaponType type, byte damage, Texture2D crosshairTexture)
        {
            _type = type;
            _damage = damage;
            _crosshairTexture = crosshairTexture;
        }
    }
}
