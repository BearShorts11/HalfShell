using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class IEnemy : MonoBehaviour
{
    public float walkSpeed = 12f;
    public float gravity = 20f;
    public float health = 100f;
    public int maxHealth = 100;
    public float damage = 10f;
    public float detectionRadius = 10;
    public float attackRaidus = 3f;
    public float attackTime = 1f; //tied to attack anim time
    public float cooldownTime = 1f;

    protected PlayerBehavior player;
    protected NavMeshAgent agent;


    protected State state;
    public enum State
    {
        idle,
        patrol,
        chasing,
        meleeAttack,
        shoot,
        cooldown
    }

    //temporary materials to show that an enemy was damaged
    public Material tempEnemNormal;
    public Material tempEnemDamage;


    // Initializes Enemy upon Start, giving them max health and grabbing the Player Object
    void Start()
    {
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerBehavior>();

        health = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        state = State.idle;
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
        player.Damage(damage);
        yield return new WaitForSeconds(attackTime);
        //could put damage here, recheck if player is within attack distance to see if they actually get damaged or not
        //aka play damage anim to give the player a chance to dodge?
        state = State.cooldown;
        StartCoroutine(Cooldown());
    }

    protected IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        agent.isStopped = false;
        state = State.chasing;
    }

    public void Damage(float damageAmt)
    {
        //override depending on enemy type??
        if (state == State.idle) state = State.chasing;

        //put in damage flash aka have a damange cooldown?
        health -= damageAmt;
        StartCoroutine(DamageFlash());

        if (health <= 0)
        {
            //Debug.Log("dead");
            Destroy(this.gameObject);
        }
    }

    protected IEnumerator DamageFlash()
    {
        gameObject.GetComponent<Renderer>().material = tempEnemDamage;
        yield return new WaitForSeconds(.5f);
        gameObject.GetComponent<Renderer>().material = tempEnemNormal;
        yield return new WaitForSeconds(.5f);
    }
}
