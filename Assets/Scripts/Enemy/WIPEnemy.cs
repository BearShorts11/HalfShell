using Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;

public abstract class WIPEnemy : MonoBehaviour, IDamageable
{
    [Header("Object pieces")]
    public PlayerBehavior Player;

    public NavMeshAgent agent;


    [Header("idk what to call this but it needs seperating")]
    [SerializeField] public float detectionRange;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackTimer;
    [SerializeField] public float damage;
    [SerializeField] public float attackCooldown { get; protected set; }
    [SerializeField] public float damageCooldown { get; protected set; }


    [Header("Health & Damage")]
    public float Health { get; set; }

    public float maxHealth { get; set; } = 50f;


    [Header("States")]
    public StateMachine stateMachine;


    // Using awake and subclasses will use start so anything in Awake is done before subclasses reach start
    void Awake()
    {
    }

    protected void Startup()
    {
        stateMachine = new StateMachine(this);
        stateMachine.Initialize(stateMachine._idleState);

        Player = FindFirstObjectByType<PlayerBehavior>();
        agent = GetComponent<NavMeshAgent>();

        Health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    public void Damage(float amount)
    {
        Debug.Log("enemy damaged");
        Health -= amount;

        //VFX
        
    }
}
