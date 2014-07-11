namespace GameFramework
{
    public static class WeaponFactory
    {
        public static Weapon CreateDefault()
        {
            return new Weapon(WeaponType.Finger);
        }

        public static Weapon CreateShotgun()
        {
            return new Weapon(WeaponType.Shotgun);
        }

        public static Weapon CreateRocketLauncher()
        {
            return new Weapon(WeaponType.Bazooka);
        }

        public static Weapon CreateFromType(WeaponType type)
        {
            Weapon weapon;

            switch (type)
            {
                case WeaponType.Shotgun:
                    weapon = CreateShotgun();
                    break;
                case WeaponType.Bazooka:
                    weapon = CreateRocketLauncher();
                    break;
            case WeaponType.Finger:
                default:
                    weapon = CreateDefault();
                    break;
            }

            return weapon;
        }
    }
}
