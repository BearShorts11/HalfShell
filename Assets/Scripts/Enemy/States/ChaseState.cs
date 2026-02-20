using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

/// <summary>
/// Handles enemy chase behavior: state where enemy will close distance between itself and the player until it is within attacking distance
/// </summary>
public class ChaseState : State
{
    //public IEnemy Owner { get; private set; }

    float attackRange;

    public ChaseState(Enemy owner)
    {
        Owner = owner;
        this.attackRange = owner.attackRange;
    }

    public override void Enter()
    {
        this.attackRange = Owner.attackRange;
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        float distanceFromPlayer = Vector3.Distance(Owner.transform.position, Owner.Player.transform.position);
        if (distanceFromPlayer <= attackRange)
        {
            //differnt chase behaviors based on what subtype of enemy is chasing the player
            switch (Owner)
            {
                case RangedEnemy:
                    Owner.stateMachine.TransitionTo(Owner.stateMachine._shootState);
                    break;
                default:
                    Owner.agent.isStopped = true;
                    Owner.stateMachine.TransitionTo(Owner.stateMachine._meleeAttackState);
                    break;
            }
        }
        else if ((Owner is RangedEnemy) && distanceFromPlayer > Owner.detectionRange)
        {
            Owner.stateMachine.TransitionTo(Owner.stateMachine._idleState);
        }
        else
        {
            Owner.agent.SetDestination(Owner.Player.transform.position);
        }
    }
}
