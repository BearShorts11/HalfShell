using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : IEnemy
{
    [SerializeField] List<Transform> waypoints;
    private int pathIndex;
    Vector3 goalPatrolPoint;
    [SerializeField] float foundTargetRadius = 2f;
    public bool isPatrol;

    private MeleeEnemy enemyLogic;
    private RagdollController ragdollController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Startup();

        if (waypoints.Count == 0 || waypoints[0] is null) isPatrol = false;
        //else isPatrol = true;

        if (isPatrol)
        {
            state = State.patrol;
            goalPatrolPoint = waypoints[0].position;
            pathIndex = 0;
        }
        //else state = State.idle;

        enemyLogic = GetComponent<MeleeEnemy>();
        animator = GetComponentInChildren<Animator>();
        ragdollController = GetComponentInChildren<RagdollController>();
    }

    // Update is called once per frame
    override public void Update()
    {
        AnimationController();

        base.Update();
        if (state == State.dead) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(state);

        switch (state)
        {
            case State.idle:
                // Detetcts if the player is within detetction radius.
                if (distanceToPlayer <= detectionRadius)
                {
                    state = State.chasing;
                }
                break;
            case State.chasing:
                base.Chase();
                break;
            case State.patrol:
                Patrol();
                break;
        }

    }

    private void Patrol()
    {
        if (state == State.dead) return;

        agent.isStopped = false;

        if (Vector3.Distance(goalPatrolPoint, transform.position) <= foundTargetRadius)
        {
            pathIndex++;
            if (pathIndex > waypoints.Count - 1) pathIndex = 0;
            if (waypoints[pathIndex] == null) Debug.Log("ERROR fill up your waypoints list (empty/null space)");

            goalPatrolPoint = waypoints[pathIndex].position;
        }
        

        agent.SetDestination(goalPatrolPoint);

        //fix
        if (Vector3.Distance(transform.position, player.transform.position) <= detectionRadius)
        {
            state = State.chasing;
        }
    }

    public override void Damage(float damageAmt)
    {
        if (state == State.dead)
        {
            ragdollController.ApplyForceToRagdoll(damageAmt);
        }
        base.Damage(damageAmt);
    }

    private void AnimationController()
    {
        //Controls Death
        if (state == State.dead)
        {
            //enemyLogic.enabled = false;
            animator.enabled = false;
            ragdollController.SetColliderState(true);
            ragdollController.SetRigidbodyState(false);
            return;
        }

        //Controls Idle/Walking/Running 
        if (state == State.idle)
        {
            animator.SetFloat("Speed", 0.0f);
        }
        else if (state == State.patrol)
        {
            animator.SetFloat("Speed", 0.5f);
        }
        else if (state == State.chasing)
        {
            animator.SetFloat("Speed", 1f);
        }
        else
        {
            animator.SetFloat("Speed", 0.0f);
        }

        //Controls Attacking
        if (state == State.meleeAttack)
        {
            animator.SetBool("Attacking", true);
            animator.Play("PunchingME");
        }
        else if (state == State.cooldown)
        {
            animator.SetBool("Attacking", false);
            animator.SetBool("Recovering", true);
        }
        else
        {
            animator.SetBool("Attacking", false);
        }

    }
}
