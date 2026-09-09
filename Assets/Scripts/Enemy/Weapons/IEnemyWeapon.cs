public interface IEnemyWeapon
{
    public enum WeaponType
    {
        Melee,
        Ranged
    }
    public int weaponDamage{ get; set; }

    public WeaponType type { get; set; }

    public int maxAmmo { get; set; }

    public float fireRate { get; set; }

    public float meleeRange { get; set; }

    public byte projPerShot { get; set; }

}
