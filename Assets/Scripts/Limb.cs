using UnityEngine;

[RequireComponent (typeof(IEnemy))]
public class Limb : MonoBehaviour
{
    public float maxHealth = 50f;
    public float health {  get; private set; }

    [SerializeField] private GameObject Gibs = null;

    [Tooltip("Multiplier for the damage taken (<1 - Less damage, >1 - More damage)")]
    [SerializeField] private float damMult = 1f; // Take more or less damage when this limb is hit
    [Tooltip("Take extra damage based on Percentage of Max HP when this limb is removed (0 - No damage,  1 - Instant Kill)")]
    [SerializeField] [Range(0, 1)] private float damMultOnRemove = 0f; // Take extra damage based on enemy max health upon removing this limb

    private IEnemy enemy;

    void Awake()
    {
        enemy = GetComponentInParent<IEnemy>();
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

        health -= Damage;

        if (enemy  != null) enemy.Damage(Damage);

        if (health <= 0f) {
            // Check again just incase
            if (enemy != null) enemy.Damage(enemy.maxHealth * damMultOnRemove);
            Destroy(gameObject);
        }
    }
}
