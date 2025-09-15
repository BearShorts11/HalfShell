using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]
public class EnemyBehavior : MonoBehaviour
{
    public float walkSpeed = 12f;
    public float gravity = 20f;
    public float health = 100f; //note to self: check the fancy shit DM was doing with health properties -N
    public int maxHealth = 100;
    public float detectionRadius = 10;
    public float attackRaidus = 2f;
    public float cooldownTime = 1f;

    private GameObject enemyObject;

    //public GameObject playerObject; //no need to have this as a class variable if it's only used once -N
    private PlayerBehavior player;
    bool alert = false;

    NavMeshAgent agent;

    private State state;

    public enum State
    {
        idle,
        chasing,
        attack,
        cooldown
    }

    //temporary materials to show that an enemy was damaged
    public Material tempEnemNormal;
    public Material tempEnemDamage;


    // Initializes Enemy upon Start, giving them max health and grabbing the Player Object
    void Start()
    {
        enemyObject = this.gameObject; //why do we need this? -N

        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerBehavior>();

        health = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        state = State.idle;
    }


    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Detetcts if the player is within detetction radius.
        // If so, persues. Enemies do not stop persuing the player post detetction
        if (distanceToPlayer <= detectionRadius)
        {
            //alert = true;
            state = State.chasing;
        }
        if (distanceToPlayer <= attackRaidus)
        {
            state = State.attack;
        }

        switch (state)
        {
            case State.idle:
                break;
            case State.chasing:
                Chase();
                break;
            case State.attack:
                Attack();
                break;
            case State.cooldown:
                StartCoroutine(Cooldown());
                break;
        }
    }

    void Chase()
    {
        if (agent != null)
        {
            agent.SetDestination(player.transform.position);
            Debug.Log("moving via agent");
        }
        else
        {
            // pretty sure agent takes care of rotation but no way to no for sure w/out a model -N
            // Rotates to "look" at the player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);

        }
    }

    void Attack()
    { 
        
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
    }

    public void Damage(float damageAmt)
    {
        //Debug.Log("ow");
        //put in damage flash aka have a damange cooldown?
        health -= damageAmt;
        StartCoroutine(DamageFlash());

        if (health <= 0)
        {
            //Debug.Log("dead");
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DamageFlash()
    {
        gameObject.GetComponent<Renderer>().material = tempEnemDamage;
        yield return new WaitForSeconds(.5f);
        gameObject.GetComponent<Renderer>().material = tempEnemNormal;
        yield return new WaitForSeconds(.5f);
    }
}
