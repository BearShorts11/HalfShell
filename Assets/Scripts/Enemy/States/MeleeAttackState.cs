using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Handles enemy melee attack behaviors: state where the enemy will make a melee attack on the player
/// </summary>
public class MeleeAttackState : State
{
    /// <summary>
    /// Attack animation length. Determines when to switch states and when to check if the player is still in range for an attack or not
    /// </summary>
    private float attackTimer;
    private bool hitPlayer;

    public MeleeAttackState(Enemy owner)
    { 
        this.Owner = owner;
        attackTimer = owner.attackTimer;
    }

    public override void Enter()
    {
        attackTimer = Owner.attackTimer;
        if (Owner.agent.isOnNavMesh) Owner.agent.isStopped = true;
        if(Owner.animator != null) Owner.animator.SetBool("Attacking", true);
        hitPlayer = false;
    }

    public override void Update()
    {
        //prevents issue with hitting player when dead?
        if (Owner.Dead)
        { 
            Owner.stateMachine.TransitionTo(Owner.stateMachine._deadState);
            return;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            //if player is still within attack range after the animation finished playing, player takes damage
            //TO REVISE: if the enemy doesn't do damage to the player have them turn instead of keep attacking the air
            if ((Owner as IHasMeleeAttack).PlayerInTrigger && !hitPlayer)
            { 
                Owner.Player.TakeDamage(Owner.damage);
                hitPlayer = true;
            }
            //kind of negates the issue of standing behind an enemy? too snappy though
            //real fix is probably only making enemy attack if player is in trigger.... ? 
            //else
            //{
            //    Owner.transform.LookAt(Owner.Player.transform);
            //}

                //automatically switch to cooldown after attack timer is done
                Owner.stateMachine._cooldownState.SetCooldownTime(Owner.attackCooldown);
                Owner.stateMachine.TransitionTo(Owner.stateMachine._cooldownState);
        }

        //melee basic never stop chasing, juggernaut can switch to his ranged attack
        if (Owner is Juggernaut)
        {
            if (Vector3.Distance(Owner.transform.position, Owner.Player.transform.position) > Owner.attackRange)
            {
                Owner.stateMachine.TransitionTo(Owner.stateMachine._chaseState);
            }
        }

    }

    public override void Exit()
    {
        if (Owner.animator != null) Owner.animator.SetBool("Attacking", false);
    }
}
