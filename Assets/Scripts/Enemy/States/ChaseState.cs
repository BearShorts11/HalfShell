using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class ChaseState : IState
{
    public WIPEnemy Owner { get; set; }

    float attackRange;

    public ChaseState(WIPEnemy owner)
    {
        Owner = owner;
        this.attackRange = owner.attackRange;
    }

    public void Enter()
    {
        this.attackRange = Owner.attackRange;
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        Debug.Log("Chase State");

        if (Vector3.Distance(Owner.transform.position, Owner.Player.transform.position) <= attackRange)
        {
            switch (Owner)
            {
                case WIPMeleeBasic:
                    Owner.stateMachine.TransitionTo(Owner.stateMachine._meleeAttackState);
                    break;
                default:
                    throw new System.Exception("case not covered: chase state");
                    break;
            }
        }
        else 
        {
            Owner.agent.SetDestination(Owner.Player.transform.position);
        }
    }
}

//playing around with subclassing states based on specific enemy types... I think this is better...
//public class JuggernautChaseState : ChaseState
//{
//    public JuggernautChaseState(WIPEnemy owner) : base(owner, )
//    { 
        
//    }
//}