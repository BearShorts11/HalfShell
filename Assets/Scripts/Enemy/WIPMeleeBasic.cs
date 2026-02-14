using UnityEngine;

public class WIPMeleeBasic : WIPEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Startup();

        detectionRange = 10f;
        attackRange = 3f;
        attackTimer = 1f;
        damage = 10f;

        agent.speed = 12f;
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }
}
