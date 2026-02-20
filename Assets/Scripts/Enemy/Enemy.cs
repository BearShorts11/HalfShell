using Assets.Scripts;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;
using static UnityEngine.UI.GridLayoutGroup;

public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Object and Component pieces")]
    public PlayerBehavior Player;
    public NavMeshAgent agent;
    public Animator animator;
    public RagdollController ragdollController;

    public GameObject BloodSplatterProjector;
    private Material[] decals;

    [Header("Designer Variables")]
    [SerializeField] public float movementSpeed = 12f;
    [SerializeField] public float detectionRange = 10f;
    [SerializeField] public float attackRange = 4f;

    /// <summary>
    /// Attack animation time. Used to determine when to switch out of the attack state and when to check if the player should be damaged or not. 
    /// </summary>
    [SerializeField] public float attackTimer = 0.5f;
    [SerializeField] public float damage = 10f;

    /// <summary>
    /// cooldown time after enemy attacks
    /// </summary>
    [SerializeField] public float attackCooldown = 0.5f;

    /// <summary>
    /// cooldown time after enemy is damaged
    /// </summary>
    [SerializeField] public float damageCooldown = 1.5f;

    [Header("Health & Damage")]
    public float Health { get; set; }
    public float maxHealth { get; set; } = 50f;


    [Header("Behavior Changes")]
    public bool Docile;

    /// <summary>
    /// spawns the enemy persuing chasing the player
    /// </summary>
    public bool SpawnAgro;


    [Header("States")]
    public StateMachine stateMachine;

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
            //different blood splatter images for variation in art. copied from the projector to here so as to not makes tons of GetComponenet calls
            decals = BloodSplatterProjector.GetComponent<DecalFadeOut>().Decals;
        }

        Health = maxHealth;
        agent.speed = movementSpeed;
        agent.acceleration = 10f;
    }

    /// <summary>
    /// Update method to be called from sub classes's Update method. For some reason Unity's MonoBehaviour Update does not run from a subclass (at least in my testing) 
    /// so anything that needs to be ran from update is called from this method in a sub class's MonoBehaviour Update() instead. 
    /// </summary>
    protected void BaseUpdate()
    {
        stateMachine.Update();

        HandleAnimation();
    }

    public virtual void TakeDamage(float amount)
    {
        //Damage is not it's own state because what Damage does depends on what state the enemy WAS in or IS CURRENTLY in
        //ex. damaging a dead enemy is going to be different from damaging a chasing enemy from damaging an idling enemy

        Health -= amount;

        animator.SetFloat("Damage", amount);
        animator.SetBool("Hit", true);

        if (Health <= 0)
        {
            stateMachine.TransitionTo(stateMachine._deadState);
            agent.enabled = false;

            StartCoroutine(SpawnDeathBloodPool());
        }

        if (stateMachine.CurrentState == stateMachine._deadState)
        {
            //move body if shot when dead
            ragdollController.ApplyForceToRagdoll(amount);
        }
        else if (stateMachine.CurrentState == stateMachine._idleState)
        {
            //make agro if damaged from far away
            stateMachine.TransitionTo(stateMachine._chaseState);
        }
        else if (stateMachine.CurrentState == stateMachine._chaseState)
        {
            //pause enemy when damaging it
            stateMachine._cooldownState.SetCooldownTime(damageCooldown);
            stateMachine.TransitionTo(stateMachine._cooldownState);
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
        //lets the enemy die & ragdoll before spawning a pool in
        yield return new WaitForSeconds(1f);

        if (BloodSplatterProjector == null) yield break; // Error prevention from having no blood splatter projector
        GameObject splatter = Instantiate(BloodSplatterProjector, this.transform.position, Quaternion.identity);
        splatter.GetComponent<DecalProjector>().material = decals[0];
        splatter.GetComponent<DecalProjector>().size = new Vector3(3, 3, 5);
        splatter.transform.Rotate(90, 0, 0);

    }

    protected void HandleAnimation()
    {
        //Controls Idle/Walking/Running 
        animator.SetFloat("VelocityX", agent.velocity.x);
        animator.SetFloat("VelocityY", agent.velocity.z);
    }


    //TODO
    /// <summary>
    /// Enemy will not attack player and will remian idle. Gets called from Debug Command ONLY
    /// </summary>
    public virtual void MakeDocile()
    {
        stateMachine.TransitionTo(stateMachine._docileState);
    }
    /// <summary>
    /// Enemy will attack player. Gets called from Debug Command ONLY
    /// </summary>
    public virtual void MakeAgro()
    { 
        stateMachine.TransitionTo(stateMachine._idleState);
        
    }
    /// <summary>
    /// Alert this enemy to the player's presence. Transition to state after Idle. Override in a sub class if the "alerted" state is not "Chasing"
    /// </summary>
    public virtual void Alert() 
    {
        stateMachine.TransitionTo(stateMachine._chaseState);
    }
}
