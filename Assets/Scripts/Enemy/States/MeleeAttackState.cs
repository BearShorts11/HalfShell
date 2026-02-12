using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackState : IState
{
    public WIPEnemy Owner { get; set; }
    private float attackTimer;

    public MeleeAttackState(WIPEnemy owner)
    { 
        this.Owner = owner;
        attackTimer = owner.attackTimer;
    }

    public void Enter()
    {
        Debug.Log("entering melee attack state");
        Debug.Log($"timer: {Owner.attackTimer}");
        attackTimer = Owner.attackTimer;
    }

    //called by state machine Update, then called from Owner object in Monobehavior Update
    public void Update()
    {
        Debug.Log("Melee Attack State");
        Owner.agent.isStopped = true;
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            if (Vector3.Distance(Owner.transform.position, Owner.Player.transform.position) <= Owner.attackRange)
            {
                Owner.Player.Damage(((WIPMeleeBasic)Owner).damage);
            }
                Owner.stateMachine._cooldownState.SetCooldownTime(Owner.attackCooldown);
                Owner.stateMachine.TransitionTo(Owner.stateMachine._cooldownState);
        }

    }

    public void Exit()
    {

    }
}
