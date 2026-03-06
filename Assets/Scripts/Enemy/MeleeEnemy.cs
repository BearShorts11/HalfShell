using UnityEngine;

public class MeleeEnemy : Enemy
{
    private void Awake()
    {
        base.Startup();
    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();
    }
}
