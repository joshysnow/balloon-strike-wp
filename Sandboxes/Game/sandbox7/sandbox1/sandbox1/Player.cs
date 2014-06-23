using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace sandbox7
{
    public class Player
    {
        private Weapon _weapon;

        public Player()
        {
            ChangeWeapon(WeaponType.Finger);
        }

        public void ChangeWeapon(WeaponType type)
        {
            switch (type)
            {
                default:
                case WeaponType.Finger:
                    _weapon = WeaponFactory.CreateDefault();
                    break;
                case WeaponType.Shotgun:
                    _weapon = WeaponFactory.CreateShotgun();
                    break;
                case WeaponType.RocketLauncher:
                    _weapon = WeaponFactory.CreateRocketLauncher();
                    break;
            }
        }

        public void UpdateInput(Vector2 lastPosition)
        {
            _weapon.UpdateInput(lastPosition);
        }

        public void Update(GameTime gameTime)
        {
            _weapon.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _weapon.Draw(spriteBatch);
        }
    }
}
