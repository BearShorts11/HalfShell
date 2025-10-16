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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerBehavior>();

        health = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        if (waypoints.Count == 0 || waypoints[0] is null) isPatrol = false;
        //else isPatrol = true;

        if (isPatrol)
        {
            state = State.patrol;
            goalPatrolPoint = waypoints[0].position;
            pathIndex = 0;
        }
        else state = State.idle;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

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
}
