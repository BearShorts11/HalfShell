using FMODUnity;
using UnityEngine;

public class MeleeEnemy : Enemy, IHasMeleeAttack
{
    [Header("Melee Enemy")]
    public bool PlayerInTrigger { get; set; }

    public bool moveWhileAttacking { get; set; } = false;

    private void Awake()
    {
        base.Startup();
    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player") PlayerInTrigger = true;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player") PlayerInTrigger = false;
    //}

    public void SetPlayerInTrigger(bool boolean)
    {
        PlayerInTrigger = boolean;
    }
}
