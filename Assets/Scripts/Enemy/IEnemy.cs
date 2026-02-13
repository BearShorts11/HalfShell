using Assets.Scripts;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class IEnemy : MonoBehaviour, IDamageable
{
    [Header("Object and Component pieces")]
    public PlayerBehavior Player;
    public NavMeshAgent agent;
    protected Animator animator;
    private RagdollController ragdollController;

    public GameObject BloodSplatterProjector;
    private Material[] decals;


    [Header("Designer Variables")]
    [SerializeField] public float detectionRange;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackTimer;
    [SerializeField] public float damage;
    /// <summary>
    /// based on animation time
    /// </summary>
    [SerializeField] public float attackCooldown = 0.5f;
    [SerializeField] public float damageCooldown = 1.5f;


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

        if (BloodSplatterProjector != null)
        {
            decals = BloodSplatterProjector.GetComponent<DecalFadeOut>().Decals;
        }

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
            agent.enabled = false;

            StartCoroutine(SpawnDeathBloodPool());
        }

        //move body if shot when dead
        if (stateMachine.CurrentState == stateMachine._deadState)
        {
            ragdollController.ApplyForceToRagdoll(amount);
        }

        //make agro if damaged from far away
        if (stateMachine.CurrentState == stateMachine._idleState)
        {
            stateMachine.TransitionTo(stateMachine._chaseState);
        }

        //VFX
        if (BloodSplatterProjector != null)
        {
            GameObject splatter = Instantiate(BloodSplatterProjector, this.transform.position, Quaternion.identity);
            splatter.GetComponent<DecalProjector>().material = decals[UnityEngine.Random.Range(2, decals.Length)];

            splatter.transform.Rotate(90, 0, 0);
        }

    }

    protected IEnumerator SpawnDeathBloodPool()
    {
        yield return new WaitForSeconds(1f);

        if (BloodSplatterProjector == null) yield break; // Error prevention from having no blood splatter projector
        GameObject splatter = Instantiate(BloodSplatterProjector, this.transform.position, Quaternion.identity);
        splatter.GetComponent<DecalProjector>().material = decals[0];
        splatter.GetComponent<DecalProjector>().size = new Vector3(3, 3, 5);
        splatter.transform.Rotate(90, 0, 0);

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
    /// <summary>
    /// Enemy will not attack player and will remian idle
    /// </summary>
    public virtual void MakeDocile() => Docile = true;
    /// <summary>
    /// Enemy will attack player
    /// </summary>
    public virtual void MakeAgro() => Docile = false;

    /// <summary>
    /// Alert this enemy to the player's presence. Transition to state after Idle. Override in a base class if the "alerted" state is not "Chasing"
    /// </summary>
    public virtual void Alert() 
    {
        stateMachine.TransitionTo(stateMachine._chaseState);
    }
}
