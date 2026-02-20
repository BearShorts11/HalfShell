using UnityEngine;

/// <summary>
/// Handles enemy idling behavior: state where the enemy does nothing until the player is within range of the enemy
/// </summary>
public class IdleState : State
{
    private float detectionRange;

    public IdleState(Enemy owner)
    {
        this.Owner = owner;
        this.detectionRange = owner.detectionRange;
    }

    public override void Enter()
    {
        this.detectionRange = Owner.detectionRange;
    }


    public override void Update()
    {
        Debug.Log("idle state");

        if (Vector3.Distance(Owner.transform.position, Owner.Player.transform.position) <= detectionRange)
        {
            //can do this, or can create subtypes of Idle class that specify a different next path
            //I think this is fine for pathing if the next path has the same behavior but if not the next other state should subclass a specified behavior for a specific enemy
            //ex. JuggernautChaseState vs. ChaseState would have different behaviors, but pathing to that can be done in this switch/case structure
            switch (Owner)
            {
                case RangedEnemy:
                    //Owner.stateMachine.TransitionTo();
                    if ((Owner as RangedEnemy).UseFirePoints)
                    {
                        
                    }
                    else
                    { 
                        Owner.stateMachine.TransitionTo(Owner.stateMachine._chaseState);
                    }
                    break;
                default:
                    //default case is Melee behavior
                    Owner.stateMachine.TransitionTo(Owner.stateMachine._chaseState);
                    break;
            }
        }
    }

    public override void Exit()
    {

    }
}
