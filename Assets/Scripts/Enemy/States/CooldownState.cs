using System.Threading;
using TMPro;
using UnityEditor.U2D.Sprites;
using UnityEngine;

/// <summary>
/// Handles enemy cooldown behavior: state where the enemy will stop and pause before continuing to the next action
/// </summary>
public class CooldownState : State
{
    private float timer;

    public CooldownState(Enemy owner)
    {
        this.Owner = owner;
        timer = 5f;
    }

    public override void Enter()
    {
        //call setcooldowntime?? or make transitioning state handle?
    }

    //called by state machine Update, then called from Owner object in Monobehavior Update
    public override void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            switch (Owner)
            {
                case RangedEnemy:
                        break;
                default:
                        Owner.agent.isStopped = false;
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
    }
}
