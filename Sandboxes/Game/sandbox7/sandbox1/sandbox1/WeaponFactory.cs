namespace sandbox7
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
    }
}
