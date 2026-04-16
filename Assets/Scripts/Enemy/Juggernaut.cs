using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Juggernaut : Enemy, IHasMeleeAttack, IHasRangedAttack
{
    [Header("Juggernaut Variables")]
    public GameObject ProjectilePrefab;

    public bool PlayerInTrigger { get; set; }

    /// <summary>
    /// when chasing, the Juggernaut will attempt to get withing melee range withing a certain period of time (this.)
    /// If not, it will attempt a ranged attack. 
    /// </summary>
    public float TimeToRangedAttack = 5f;

    /// <summary>
    /// the next time the Juggernaut will be able to make a ranged attack
    /// </summary>
    public float nextTimeToRangedAttack { get; private set; }

    private void Start()
    {
        //base.Startup();

        Health = maxHealth;
        agent.speed = movementSpeed;
        agent.acceleration = 10f;

        stateMachine = new StateMachine(this);
        stateMachine.Initialize(stateMachine._idleState);

        Player = FindFirstObjectByType<PlayerBehavior>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        //base.BaseUpdate();

        stateMachine.Update();
        Debug.Log(stateMachine.CurrentState);
    }


    public override void TakeDamage(float amount)
    {
        Health -= amount;

        Debug.Log("ouchie " + Health);

        if (Health <= 0 & Dead == false)
        {
            stateMachine.TransitionTo(stateMachine._deadState);
            agent.enabled = false;
            Dead = true;

            Destroy(gameObject);
            //StartCoroutine(SpawnDeathBloodPool());
        }

        if (stateMachine.CurrentState == stateMachine._idleState)
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
    }

    public void SetNextRangedAttackTime()
    { 
        nextTimeToRangedAttack = Time.time + TimeToRangedAttack;
    }

    public override void Shoot()
    {
        //animate

        //get object from the pool (eventually)
        GameObject bullet = GameObject.Instantiate(ProjectilePrefab, this.transform.position + this.transform.forward, this.transform.rotation, this.transform);
        bullet.SetActive(true);
        //set transform to that of enemy's gun (seperated for pooling)
        //bullet.transform.position = hand position
        //bullet.transform.rotation = hand position
        //bullet.transform.parent = transform;

        Vector3 playerCurrPos = Player.transform.position;
        bullet.GetComponent<EnemyBullet>().GiveTarget(playerCurrPos);

        //play sound
        //RuntimeManager.PlayOneShot(firingSound, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") PlayerInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") PlayerInTrigger = false;
    }

    public void SetPlayerInTrigger(bool boolean)
    {
        PlayerInTrigger = boolean;
    }
}