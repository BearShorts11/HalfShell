using Assets.Scripts;
using System.Collections;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
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
    public GameObject FullyGibbedParticle;

    [SerializeField] GameObject HealthPackDrop;

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
    [field: SerializeField] public float maxHealth { get; set; } = 50f;
    public bool Dead { get; set; }

    /// <summary>
    /// if the enemy is effected by any staus effects from shells
    /// </summary>
    private bool statusEffected;

    /// <summary>
    /// holds a reference to the shell if effected by a status effect in order to access all it
    /// </summary>
    private ShellBase statusEffectShell;

    /// <summary>
    /// if effected by a status effect, next time to take damage on
    /// </summary>
    private float timeToStatusDamage;

    /// <summary>
    /// used to prevent triggering hit animation if damaged from a status effect
    /// </summary>
    private bool damageFromStatusEffect;

    /// <summary>
    /// cooldown on if status effected && hit from a half shell || slug so that enemy doesn't drop a million health packs from being hit from a slug
    /// </summary>
    private float specialHealthDropCooldown = 0.5f;
    private float nextTimeToSpecialHealthDrop;

    private ShellBase lastHitFrom;

    [Header("Behavior Changes")]
    public bool Docile;

    /// <summary>
    /// spawns the enemy persuing chasing the player
    /// </summary>
    public bool SpawnAgro;

    /// <summary>
    /// enemy will not return to idle even if player is outside of detection radius, instead constantly persuing the player
    /// </summary>
    public bool AlwaysChase;


    [Header("States")]
    public StateMachine stateMachine;

    [Header("Unity Event Stuff")]
    [SerializeField] public UnityEvent OnDeath;
    [SerializeField] public static UnityEvent DeathAlert = new UnityEvent();

    [Header("Sounds")]
    [SerializeField] public SimpleSoundEvent soundEvents;
    protected float defaultVocalCoolDown = 3;
    protected float vocalCoolDown;
    protected float lastVocalization;

    protected void Startup()
    {
        stateMachine = new StateMachine(this);

        Player = FindFirstObjectByType<PlayerBehavior>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        ragdollController = GetComponentInChildren<RagdollController>();

        if (SpawnAgro)
        {
            AlwaysChase = true;
            stateMachine.Initialize(stateMachine._chaseState);
        } 
        else stateMachine.Initialize(stateMachine._idleState);

        if (BloodSplatterProjector != null)
        {
            //different blood splatter images for variation in art. copied from the projector to here so as to not makes tons of GetComponenet calls
            decals = BloodSplatterProjector.GetComponent<DecalFadeOut>().Decals;
        }

        Health = maxHealth;
        agent.speed = movementSpeed;
        // Why? - V 
        //acceleration effects turn speed, so they don't go fucking flying past you when trying to hit you
        agent.acceleration = 10f;

        if (soundEvents == null)
            soundEvents = this.gameObject.GetComponent<SimpleSoundEvent>();
        vocalCoolDown = defaultVocalCoolDown;
    }

    /// <summary>
    /// Update method to be called from sub classes's Update method. For some reason Unity's MonoBehaviour Update does not run from a subclass (at least in my testing) 
    /// so anything that needs to be ran from update is called from this method in a sub class's MonoBehaviour Update() instead. 
    /// </summary>
    protected void BaseUpdate()
    {
        stateMachine.Update();

        HandleAnimation();

        //Debug.Log(statusEffected);
        if (statusEffected && Time.time > timeToStatusDamage)
        {
            //could have a custom method to take damage if we don't want the hit animation to play every time
            damageFromStatusEffect = true;
            TakeDamage(statusEffectShell.effectDamage);
            timeToStatusDamage = Time.time + statusEffectShell.effectHitPerSecond;
        }
    }

    //not abstract so melee doesn't have to implement it, but should be
    //actually should probably be an interface "IRangedAttack"
    public virtual void Shoot() { }

    //could move to death state but it's the difference between chekcing every frame vs. checking when the body actually takes damage
    public virtual void TakeDamage(float amount)
    {
        //Damage is not it's own state because what Damage does depends on what state the enemy WAS in or IS CURRENTLY in
        //ex. damaging a dead enemy is going to be different from damaging a chasing enemy from damaging an idling enemy

        Health -= amount;

        //only trigger animation if the damage was NOT from a status effect
        //very much could cause errors, but this is the best solution without editing the parameters.
        if (!damageFromStatusEffect)
        {
            animator.SetFloat("Damage", amount);
            if (!IsInvoking(nameof(StopHitReaction)))
            {
                animator.SetBool("Hit", true);
                Invoke(nameof(StopHitReaction), 1);
            }
        }

        //checking for dead prevents this from firing every time the enemy is shot after death
        if (Health <= 0 & Dead == false)
        {
            stateMachine.TransitionTo(stateMachine._deadState);
            agent.enabled = false;
            Dead = true;
            //OnDeath?.Invoke();

            //put here instead of dead state so that it doesn't trigger when loading a save
            Enemy.DeathAlert.Invoke();

            StartCoroutine(SpawnDeathBloodPool());
        }

        if (statusEffected && !Dead && (lastHitFrom.Type != ShellBase.ShellType.Incindiary) && (Time.time > nextTimeToSpecialHealthDrop))
        {
            //drop health pack
            GameObject drop = Instantiate(HealthPackDrop);
            drop.GetComponent<HealthPickup>().regainAmount = 5;
            nextTimeToSpecialHealthDrop = Time.time + specialHealthDropCooldown;
        }

        //behavior based on state
        if (stateMachine.CurrentState == stateMachine._deadState)
        {
            //move body if shot when dead
            ragdollController.ApplyForceToRagdoll(amount + 0.1f); //+ 0.1 prevents issue of ragdoll FLYING if amt = 0
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

        if (Health <= -(maxHealth * 2) && FullyGibbedParticle != null) // Enemy/Corpse took a lot of damage than twice it's max HP, turn into mist completely
        {
            FullyGibbedParticle.SetActive(true);
            FullyGibbedParticle.gameObject.transform.parent = null;
            //Destroy(this.gameObject);
            this.gameObject.SetActive(false);

            //Player.GetComponent<Kerth>().GibbedEnemy(this);

            //TODO: add to kerth list like pickups
            //re-enable on reload

            return;
        }

        damageFromStatusEffect = false;
    }

    public void HitEffect(ShellBase shell)
    {
        Debug.Log("oh no i'm effected by a status");

        statusEffected = true;
        statusEffectShell = shell;

        damageFromStatusEffect = true;
        //TakeDamage(statusEffectShell.effectDamage);

        timeToStatusDamage = Time.time + shell.effectHitPerSecond;
        StartCoroutine(EffectOverIn(shell.effectTime));
    }

    public IEnumerator EffectOverIn(float time)
    {
        yield return new WaitForSeconds(time);
        statusEffected = false;
    }

    /// <summary>
    /// undo death actions when reloading
    /// </summary>
    public void Revive()
    {
        Debug.Log("reviving enemy");

        this.agent.enabled = true;
        if (this.agent.isOnNavMesh) this.agent.isStopped = false;
        Dead = false;

        animator.enabled = true;
        ragdollController.SetColliderState(false);
        ragdollController.SetRigidbodyState(true);
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

    /// <summary>
    /// Handles run/walking animations
    /// </summary>
    protected void HandleAnimation()
    {
        //Controls Idle/Walking/Running 
        animator.SetFloat("Velocity X", agent.velocity.x);
        animator.SetFloat("Velocity Y", agent.velocity.z);
    }

    protected void StopHitReaction()
    {
        animator.SetBool("Hit", false);
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
        AlwaysChase = true;
        stateMachine.TransitionTo(stateMachine._chaseState);
    }

    /// <summary>
    /// Alert this enemy to the player's presence. Transition to state after Idle. Override in a sub class if the "alerted" state is not "Chasing"
    /// </summary>
    public virtual void Alert() 
    {
        AlwaysChase = true;
        stateMachine.TransitionTo(stateMachine._chaseState);
    }

    //https://stackoverflow.com/questions/33437244/find-children-of-children-of-a-gameobject
    /// <summary>
    /// Finds the child of the given Transform of the given name. Used to find the Pistol object for this enemy in order to spawn a bullet accurately
    /// where the gun object is. 
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="childName"></param>
    /// <returns></returns>
    public Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    protected void PlayVoice(string eventPath)
    {
        if (soundEvents != null && Health > 0)
        {
            soundEvents.PlaySoundAttached(eventPath); 
            lastVocalization = Time.time;
        }
    }

    protected bool IsOnVocalCooldown()
    {
        return Time.time < lastVocalization + vocalCoolDown;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void SetHealth(float health) => Health = health;

    /// <summary>
    /// update what the enemy was hit from. Used to check for special effect drops
    /// </summary>
    /// <param name="shell"></param>
    public void HitFrom(ShellBase shell) => lastHitFrom = shell;
}
