using Assets.Scripts;
using UnityEngine;

public class Limb : MonoBehaviour, IDamageable
{
    [Tooltip("The max amount health set")]
    public float defaultHealth = 50f;
    public float maxHealth { get; set; }
    [field: SerializeField] public float Health {  get; set; }

    [Tooltip("Should the limb have it's own health or pass down the damage to the enemy instead. NOTE: damMult is still in effect")]
    public bool isRemovable = true;

    [Tooltip("Is this Game Object a separate object or a part of the armature? Shrinks the parent after this limb has been removed")] // Attach a bloody stump afterwards? idk
    [SerializeField] private bool isAttatchedToBone = false;

    [Tooltip("(Requires isAttatchedToBone = true) Is this the bone collider itself?")]
    [SerializeField] private bool isBoneItself = false;

    [Tooltip("(Requires isRemovable = false) Should this limb be removal after the enemy is dead?")]
    [SerializeField] private bool removableAfterDeath = false;

    [Tooltip("Is this soley a hitbox or is this a game object that relies on physics?")]
    [SerializeField] private bool hasCollision = false;

    [Tooltip("Prefab that is used to set an inactive prefab in the scene active and play the gibbing effects. Can be used to represent debris as well or so.")]
    [SerializeField] private GameObject Gibs = null;

    [Tooltip("Multiplier for the damage taken (<1 - Less damage, >1 - More damage)")]
    [SerializeField] private float damMult = 1f; // Take more or less damage when this limb is hit
    [Tooltip("Take extra damage based on Percentage of Max HP when this limb is removed (0 - No damage,  1 - Instant Kill)")]
    [SerializeField] [Range(0, 1)] private float damPctHealthOnRemove = 0f; // Take extra damage based on enemy max health upon removing this limb

    private Enemy enemy;
    private Collider coll;

    void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        if (enemy == null)
            Debug.LogError("Error! Enemy script not detected! Is this Game Object a child of the Enemy Game Object?");
        coll = this.gameObject.GetComponent<Collider>();
        if (coll == null)
            Debug.LogError("Error! No collider attatched to this script's Game Object!");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = defaultHealth;
        Health = maxHealth;
        if (removableAfterDeath && isRemovable)
        {
            Debug.LogWarning($"Warning, removableAfterDeath is enabled with isRemovable, use one of the two!\nisRemovable has been automatically turned off");
            isRemovable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // (hasCollision) If this is just a hitbox and not a collider for the ragdoll system, disable collision
        // (coll.isTrigger) If it's also trigger, keep the collision enabled.
        // If this confuses you, the code after the if statement means { if (!hasCollision) { if (coll.isTrigger){ } else { } } else { } }
        // This code is here because of the ragdoll system.
        if (enemy.Health <= 0 && coll.enabled) hasCollision = false ? coll.isTrigger = true ? coll.enabled = true : coll.enabled = false : coll.enabled = true;
    }

    public void Damage(float Damage) // Should be called by the shotgun or any other source that would damage this
    {
        if (Health <= 0f) return; // Do not run the code if the limb is already at 0 health! Fixes the strange issue of this funciton being called multiple times from being hit by a half shell (Multi-hit, so it makes sense that it would do that I guess?) 
        Damage *= damMult;

        if (isRemovable || (enemy.Health - Damage <= 0 && removableAfterDeath)) 
            Health -= Damage;       // This limb can only lose health if it is removable (i.e decapitation, amputation, is a body armor, etc.)
                                    //  This also means that the health check in the last line will never return true

        // Take *some* damage but never enough damage to no more than 25% of the limb's maximum health if this is only
        // Removable on death.
        if (!isRemovable && (enemy.Health - Damage >= maxHealth * .25f && removableAfterDeath))
        {
            Health = Mathf.Clamp(enemy.Health - Damage, maxHealth * .25f, enemy.maxHealth);
        }

        if (enemy  != null) enemy.Damage(Damage); // Pass the damage down to the enemy

        if (Health <= 0f && (isRemovable || (enemy.Health <= 0 && removableAfterDeath)))
        {
            // Check again just incase (Since the enemy took damage prior to this check, include the damage here to consider how much the enemy had prior)
            if (enemy != null && enemy.Health + Damage > 0 && damPctHealthOnRemove > 0) enemy.Damage(enemy.maxHealth * damPctHealthOnRemove);
            if (isAttatchedToBone) {
                if (isBoneItself)
                    transform.localScale = Vector3.zero;
                else
                { 
                    transform.parent.localScale = Vector3.zero;
                    //transform.parent.GetComponent<Collider>().enabled = false;
                }
            }
            if (Gibs != null) { 
                //Instantiate(Gibs, this.gameObject.transform.position, Quaternion.identity); 
                if (!Gibs.activeInHierarchy) Gibs.SetActive(true);
            }
            if (isBoneItself)
            { 
                Destroy(this);
                return;
            }
            Destroy(gameObject);
        }
    }
}
