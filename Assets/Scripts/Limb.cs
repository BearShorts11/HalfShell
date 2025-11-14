using UnityEngine;

public class Limb : MonoBehaviour
{
    public float maxHealth = 50f;
    public float health {  get; private set; }

    [Tooltip("Should the limb have it's own health or pass down the damage to the enemy instead. NOTE: damMult is still in effect")]
    public bool isRemovable = true;

    [Tooltip("Is this a separate object or a part of the armature? Shrinks the bone after this limb has been removed")] // Attach a bloody stump afterwards? idk
    [SerializeField] private bool isBone = false;

    // Not used yet, could be a particle system game object or something that acts like it
    [SerializeField] private GameObject Gibs = null;

    [Tooltip("Multiplier for the damage taken (<1 - Less damage, >1 - More damage)")]
    [SerializeField] private float damMult = 1f; // Take more or less damage when this limb is hit
    [Tooltip("Take extra damage based on Percentage of Max HP when this limb is removed (0 - No damage,  1 - Instant Kill)")]
    [SerializeField] [Range(0, 1)] private float damPctHealthOnRemove = 0f; // Take extra damage based on enemy max health upon removing this limb

    private IEnemy enemy;

    void Awake()
    {
        enemy = GetComponentInParent<IEnemy>();
        if (enemy == null)
            Debug.LogError("Error! Enemy script not detected! Is this Game Object a child of the Enemy Game Object?");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float Damage) // Should be called by the shotgun or any other source that would damage this
    {
        Damage *= damMult;

        if (isRemovable) health -= Damage;  // This limb can only lose health if it is removable (i.e decapitation, amputation, is a body armor, etc.)
                                            //  This also means that the health check in the last line will never return true

        if (enemy  != null) enemy.Damage(Damage); // Pass the damage down to the enemy

        if (health <= 0f && isRemovable) {
            // Check again just incase
            if (enemy != null && damPctHealthOnRemove > 0) enemy.Damage(enemy.maxHealth * damPctHealthOnRemove);
            if (isBone) transform.parent.localScale = Vector3.zero;
            Destroy(gameObject);
        }
    }
}
