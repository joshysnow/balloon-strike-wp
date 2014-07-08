namespace sandbox9
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
            return new Weapon(WeaponType.RocketLauncher);
        }

        public static Weapon CreateFromType(WeaponType type)
        {
            Weapon weapon;

            switch (type)
            {
                case WeaponType.Shotgun:
                    weapon = CreateShotgun();
                    break;
                case WeaponType.RocketLauncher:
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
