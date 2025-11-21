using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Assets.Scripts;

public class IEnemy : MonoBehaviour, IDamageable
{
    public float walkSpeed = 12f;
    public float gravity = 20f;

    [Tooltip("The max amount health set")]
    public float defaultHealth = 50f; // Since you can't set default values in properties
    public float maxHealth { get; set; }
    [field: SerializeField] public float Health { get; set; }
    public float damage = 10f;
    public float detectionRadius = 10;
    public float attackRaidus = 3f;
    public float attackTime = 1f; //tied to attack anim time
    public float attackCooldownTime = 0.5f;
    public float damageCooldownTime = 1.5f;

    protected PlayerBehavior player;
    protected NavMeshAgent agent;
    protected Animator animator;

    public bool SpawnAgro;
    protected State state;
    protected State startState = State.idle;
    public enum State
    {
        idle,
        patrol,
        chasing,
        findCover,
        meleeAttack,
        shoot,
        cooldown,
        dead
    }

    //temporary materials to show that an enemy was damaged
    public Material tempEnemNormal;
    public Material tempEnemDamage;

    private void Awake()
    {
        if (SpawnAgro) startState = State.chasing;
    }

    // Initializes Enemy upon Start, giving them max health and grabbing the Player Object
    void Start()
    {
        Startup();
    }

    /// <summary>
    /// call from base classes in Start() so as to not copy/paste code
    /// </summary>
    protected void Startup()
    {
        Debug.Log("starting up");
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerBehavior>();

        animator = player.GetComponentInChildren<Animator>();

        maxHealth = defaultHealth;

        Health = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        state = startState;
    }

    void Update()
    {
        
    }

    protected void Chase()
    {
        if (agent != null)
        {
            agent.SetDestination(player.transform.position);

            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= attackRaidus)
            {
                state = State.meleeAttack;
                StartCoroutine(Attack());
            }
        }
        else
        {
            //lowkey don't need this

            // Rotates to "look" at the player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
        }
    }

    protected IEnumerator Attack()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(attackTime);
        //could put damage here, recheck if player is within attack distance to see if they actually get damaged or not
        //aka play damage anim to give the player a chance to dodge?
        player.Damage(damage);
        state = State.cooldown;
        StartCoroutine(Cooldown(attackCooldownTime));
    }

    protected IEnumerator Cooldown(float time)
    {
        //this is how you do a full stop. for some reason just one of these does not work. all 3 however? yeah apparently that works
        //agent.speed = 0;
        //agent.isStopped = true;
        //agent.SetDestination(transform.position);


        yield return new WaitForSeconds(time);

        agent.speed = walkSpeed;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= attackRaidus)
        {
            state = State.meleeAttack;
            StartCoroutine(Attack());
        }
        else
        {
            agent.isStopped = false;
            state = State.chasing;
        }
    }

    
    public void Damage(float damageAmt)
    {
        //put in damage flash aka have a damange cooldown?
        Health -= damageAmt;

        SwitchStateOnDamage();
        StartCoroutine(DamageFlash());

        if (Health <= 0)
        {
            //Debug.Log("dead");
            state = State.dead; 
        }
    }

    protected void SwitchStateOnDamage()
    {
        //override depending on enemy type??
        if (state == State.idle || state == State.patrol) state = State.chasing;
        if (state == State.chasing)
        { 
            state = State.cooldown;
            StartCoroutine(Cooldown(damageCooldownTime));
        }
        else if (state == State.dead)
        {
            StopAllCoroutines();
            Destroy(this.gameObject, 10f);
        }
    }

    protected IEnumerator DamageFlash()
    {
        gameObject.GetComponent<Renderer>().material = tempEnemDamage;
        yield return new WaitForSeconds(.5f);
        gameObject.GetComponent<Renderer>().material = tempEnemNormal;
        yield return new WaitForSeconds(.5f);
    }

    //can call from editor by right clicking script- for debugging
    [ContextMenu("Damage")]
    public void DamageTest()
    {
        Damage(10f);
    }

    public void SwitchState(State newState) => state = newState;
    public void Alert() => state = State.chasing;
    public State GetState() => state;
    public void SetStartState(State state) => this.startState = state;
}
