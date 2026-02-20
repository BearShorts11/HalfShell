using UnityEngine;
using UnityEditor;

/// <summary>
/// Handles enemy death behavior: state where the enemy performs death actions
/// </summary>
public class DeadState : State
{

    public DeadState(Enemy owner)
    { 
        this.Owner = owner;
    }

    public override void Enter()
    {
        //stops character from moving, doesn't need to happen every frame in update
        if (Owner.agent.isActiveAndEnabled) Owner.agent.isStopped = true;
        Object.Destroy(Owner.gameObject, 10f);

        Owner.animator.enabled = false;
        Owner.ragdollController.SetColliderState(true);
        Owner.ragdollController.SetRigidbodyState(false);


        //handle enemy drops
        if (Owner.Player.Health <= 25)
        {
            GameObject healthPickup = Owner.RecursiveFindChild(Owner.transform, "Small Health Pack Variant").gameObject;
            if (healthPickup != null) healthPickup.SetActive(true);
        }

        switch (Owner)
        {
            case RangedEnemy:
                Slug s = new Slug();
                if (Owner.Player.GetComponent<PlayerShooting>().AmmoCounts[ShellBase.ShellType.Slug]
                    <= s.MaxHolding / 5)
                {
                    GameObject ammoBox = Owner.RecursiveFindChild(Owner.transform, "Small Ammo RB").gameObject;  
                    if (ammoBox != null) ammoBox.SetActive(true);
                }
                break;
        }
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {

        //handle death
        //call methods from owner to do all the things
        
    }
}
