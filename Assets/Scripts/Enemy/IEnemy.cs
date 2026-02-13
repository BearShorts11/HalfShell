using Assets.Scripts;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class IEnemy : MonoBehaviour, IDamageable
{
    [Header("Object and Component pieces")]
    public PlayerBehavior Player;
    public NavMeshAgent agent;
    protected Animator animator;
    private RagdollController ragdollController;


    [Header("Designer Variables")]
    [SerializeField] public float detectionRange;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackTimer;
    [SerializeField] public float damage;
    [SerializeField] public float attackCooldown { get; protected set; }
    [SerializeField] public float damageCooldown { get; protected set; }


    [Header("Health & Damage")]
    public float Health { get; set; }
    public float maxHealth { get; set; } = 50f;


    [Header("Behavior Changes")]
    public bool SpawnAgro;
    public bool Docile;


    [Header("States")]
    public StateMachine stateMachine;


    // Using awake and subclasses will use start so anything in Awake is done before subclasses reach start
    void Awake()
    {
    }

    protected void Startup()
    {
        stateMachine = new StateMachine(this);

        if (SpawnAgro) stateMachine.Initialize(stateMachine._chaseState);
        else stateMachine.Initialize(stateMachine._idleState);

        Player = FindFirstObjectByType<PlayerBehavior>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        ragdollController = GetComponentInChildren<RagdollController>();

        Health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void BaseUpdate()
    {
        stateMachine.Update();

        HandleAnimation();
    }

    public void Damage(float amount)
    {
        Debug.Log("enemy damaged");
        Health -= amount;

        animator.SetFloat("Damage", amount);
        animator.SetBool("Hit", true);

        if (Health <= 0)
        {
            stateMachine.TransitionTo(stateMachine._deadState);
        }

        //make agro if damaged from far away
        if (stateMachine.CurrentState == stateMachine._idleState)
        {
            stateMachine.TransitionTo(stateMachine._chaseState);
        }

        //VFX

    }

    protected void HandleAnimation()
    {
        //Controls Death
        if (stateMachine.CurrentState == stateMachine._deadState)
        {
            animator.enabled = false;
            ragdollController.SetColliderState(true);
            ragdollController.SetRigidbodyState(false);
            return;
        }

        //Controls Idle/Walking/Running 
        animator.SetFloat("VelocityX", agent.velocity.x);
        animator.SetFloat("VelocityY", agent.velocity.z);


        //Controls Attacking
        switch (stateMachine.CurrentState)
        {
            case MeleeAttackState:
                animator.SetBool("Attacking", true);
                    break;
            default:
                animator.SetBool("Attacking", false);
                break;
        }

    }


    //TODO
    public virtual void MakeDocile() { }

    public virtual void MakeAgro() { }
}
