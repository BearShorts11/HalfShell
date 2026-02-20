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

    public MeleeAttackState(Enemy owner)
    { 
        this.Owner = owner;
        attackTimer = owner.attackTimer;
    }

    public override void Enter()
    {
        attackTimer = Owner.attackTimer;
        Owner.agent.isStopped = true;
        Owner.animator.SetBool("Attacking", true);
    }

    public override void Update()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            //if player is still within attack range after the animation finished playing, player takes damage
            //TO REVISE: see if we can't add a collider on the player to trigger damage instead... 

            /**
             *  TODO: if you want to avoid player taking damage when they are behind the enemy or to it's side, but still within the attack range, 
             *  you could turn on a collider for whatever frames of the attack animation look like they are dealing damage, and check if that collider is intersecting the player.
             *  If you did that, I would consider using animation events instead of trying to time it so that you can have a visual inspector to know which frames should deal damage.
             */

            if (Vector3.Distance(Owner.transform.position, Owner.Player.transform.position) <= Owner.attackRange)
            {
                Owner.Player.TakeDamage(((MeleeEnemy)Owner).damage);
            }
            //automatically switch to cooldown after attack timer is done
                Owner.stateMachine._cooldownState.SetCooldownTime(Owner.attackCooldown);
                Owner.stateMachine.TransitionTo(Owner.stateMachine._cooldownState);
        }

    }

    public override void Exit()
    {
        Owner.animator.SetBool("Attacking", false);
    }
}
