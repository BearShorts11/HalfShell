using UnityEngine;

public class MeleeEnemy : Enemy
{
    public bool PlayerInTrigger;

    private void Awake()
    {
        base.Startup();
    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") PlayerInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") PlayerInTrigger = false;
    }
}
