using UnityEngine;
using UnityEngine.AI;

public class MeleeAttackState : IState
{
    public IEnemy Owner { get; set; }
    private float attackTimer;

    public MeleeAttackState(IEnemy owner)
    { 
        this.Owner = owner;
        attackTimer = owner.attackTimer;
    }

    public void Enter()
    {
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
                Owner.Player.Damage(((MeleeEnemy)Owner).damage);
            }
                Owner.stateMachine._cooldownState.SetCooldownTime(Owner.attackCooldown);
                Owner.stateMachine.TransitionTo(Owner.stateMachine._cooldownState);
        }

    }

    public void Exit()
    {

    }
}
