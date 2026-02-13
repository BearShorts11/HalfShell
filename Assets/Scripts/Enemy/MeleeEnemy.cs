using UnityEngine;

public class MeleeEnemy : IEnemy
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
        agent.acceleration = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();
    }
}
