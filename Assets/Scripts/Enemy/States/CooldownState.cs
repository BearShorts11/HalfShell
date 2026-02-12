using System.Threading;
using UnityEditor.U2D.Sprites;
using UnityEngine;

public class CooldownState : IState
{
    public WIPEnemy Owner { get; set; }
    private float timer;

    public CooldownState(WIPEnemy owner)
    {
        this.Owner = owner;
        timer = 5f;
    }

    public void Enter()
    {
        //call setcooldowntime?? or make transitioning state handle?
    }

    //called by state machine Update, then called from Owner object in Monobehavior Update
    public void Update()
    {
        Debug.Log("Cooldow State");
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            switch (Owner)
            {
                case WIPMeleeBasic:

                    //if (Vector3.Distance(Owner.transform.position, Owner.Player.transform.position) < Owner.attackRange)
                    //{
                    //    Owner.stateMachine.TransitionTo(Owner.stateMachine._meleeAttackState);
                    //}
                    //else
                    //{
                    //}
                        Owner.stateMachine.TransitionTo(Owner.stateMachine._chaseState);

                        break;
                default:
                    throw new System.Exception("no case defined: cooldown");
                    break;
            }
        }
    }

    public void Exit()
    {
        
    }

    //since cooldown could be used for a lot of things, have this to give it a specific time
    public void SetCooldownTime(float time)
    { 
        timer = time;
    }
}
