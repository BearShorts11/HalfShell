using UnityEngine;

public class IdleState : IState
{
    public IEnemy Owner { get; set; }

    private float detectionRange;

    public IdleState(IEnemy owner)
    {
        this.Owner = owner;
        this.detectionRange = owner.detectionRange;
    }

    public void Enter()
    {
        this.detectionRange = Owner.detectionRange;
    }

    //called by state machine Update, then called from Owner object in Monobehavior Update
    public void Update()
    {
        Debug.Log("Idle State");

        if (Vector3.Distance(Owner.transform.position, Owner.Player.transform.position) <= detectionRange)
        {
            //Debug.Log("player detected");

            //can do this, or can create subtypes of Idle class that specify a different next path
            //I think this is fine for pathing if the next path has the same behavior but if not the next other state should subclass a specified behavior for a specific enemy
            //ex. JuggernautChaseState vs. ChaseState would have different behaviors, but pathing to that can be done in this switch/case structure
            switch (Owner)
            {
                case MeleeEnemy:
                    Owner.stateMachine.TransitionTo(Owner.stateMachine._chaseState);
                    break;
                default:
                    throw new System.Exception("case not covered: idle state");
                    break;
            }
        }
    }

    public void Exit()
    {

    }
}
