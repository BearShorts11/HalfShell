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
