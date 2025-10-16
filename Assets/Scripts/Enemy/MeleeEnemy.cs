using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemy : IEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerBehavior>();

        health = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;

        state = State.idle;
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
            case State.meleeAttack:
                break;
            case State.cooldown:
                break;
        }
    }
}
