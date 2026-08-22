using System.Threading;
using TMPro;
using UnityEngine;

/// <summary>
/// Handles enemy cooldown behavior: state where the enemy will stop and pause before continuing to the next action
/// </summary>
public class CooldownState : State
{
    private float timer;
    private float setTime;

    public CooldownState(Enemy owner)
    {
        this.Owner = owner;
        timer = 5f;
        setTime = timer;
    }

    public override void Enter()
    {
        //call setcooldowntime?? or make transitioning state handle?
    }

    //called by state machine Update, then called from Owner object in Monobehavior Update
    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer > setTime * 0.5f && (Owner as IHasMeleeAttack).moveWhileAttacking)
            if (Owner.agent.isActiveAndEnabled && Owner.agent.isOnNavMesh) Owner.agent.SetDestination(Owner.Player.transform.position);

        if (timer <= 0)
        {
            switch (Owner)
            {
                case RangedEnemy:
                        break;
                default:
                        if (Owner.agent.isActiveAndEnabled && Owner.agent.isOnNavMesh) Owner.agent.isStopped = false;
                        Owner.stateMachine.TransitionTo(Owner.stateMachine._chaseState);
                    break;
            }
        }
    }

    public override void Exit()
    {
        
    }

    //since cooldown could be used for a lot of things, have this to give it a specific time
    public void SetCooldownTime(float time)
    { 
        timer = time;
        setTime = time;
    }
}
