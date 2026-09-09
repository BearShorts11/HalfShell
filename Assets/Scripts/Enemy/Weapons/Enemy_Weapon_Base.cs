using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

// A class that holds information for future enemy classes to access, should it be desired in a rewrite/rework later down the road.
// as of now, this is accessible to the mannequin enemies
public class Enemy_Weapon_Base : MonoBehaviour, IEnemyWeapon
{
    [field: SerializeField] public int weaponDamage { get; set; } = 0;
    [field: SerializeField] public IEnemyWeapon.WeaponType type { get; set; } = IEnemyWeapon.WeaponType.Melee;
    [field: SerializeField] public int maxAmmo { get; set; } = 0;
    [field: SerializeField] public float fireRate { get; set; } = 0.1f;
    [field: SerializeField] public byte projPerShot { get; set; } = 1;
    [field: SerializeField] public float meleeRange { get; set; } = 1;

    public float pickupRange = 10f;

    [field: SerializeField] public Enemy owner { get; protected set; }

    [field: SerializeField] public EventReference firingSound { get; protected set; }

    private Rigidbody rb;

    private Collider[] col;

    public void Start()
    {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody component))
        {
            rb = component;
        }
        else
            rb = gameObject.GetComponentInChildren<Rigidbody>();

        col = gameObject.GetComponentsInChildren<Collider>();
    }

    virtual public void Drop()
    {
        Drop(new(0, 0, 0), new(0, 0, 0));
    }

    virtual public void Drop(Vector3 velocity, Vector3 angVel)
    {
        if (owner)
        {
            //this.transform.SetParent(null, true);
            this.transform.parent = null;
            if (rb)
            {
                rb.isKinematic = false;
                EnableCollision(true);
                rb.linearVelocity = velocity;
                rb.angularVelocity = angVel;
            }
            owner = null;
        }
    }

    virtual public void Grabbed(Enemy enemy)
    {
        owner = enemy;
        rb.isKinematic = true;
        EnableCollision(false);
    }

    private void EnableCollision(bool bEnable)
    {
        if (col.Length > 0)
            foreach(Collider c in col)
                c.enabled = bEnable;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(this.transform.position, pickupRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, meleeRange);
    }
}
