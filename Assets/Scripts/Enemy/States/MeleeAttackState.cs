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

    Juggernaut ownerJuggernaut;
    MannequinEnemy ownerMannequin;

    public MeleeAttackState(Enemy owner)
    { 
        this.Owner = owner;
        attackTimer = owner.attackTimer;
        ownerJuggernaut = owner as Juggernaut;
        ownerMannequin = owner as MannequinEnemy;
    }

    public override void Enter()
    {
        attackTimer = Owner.attackTimer;
        if (Owner.agent.isOnNavMesh && ownerMannequin is null) Owner.agent.isStopped = true;
        if (Owner.animator != null) Owner.animator.SetTrigger("Attacking");
        hitPlayer = false;

        if (ownerJuggernaut is not null)
        {
            ownerJuggernaut.soundEvents.PlaySoundAttached(ownerJuggernaut.meleeSwingSound);
        }
    }

    public override void Update()
    {
        //prevents issue with hitting player when dead?
        if (Owner.Dead)
        { 
            Owner.stateMachine.TransitionTo(Owner.stateMachine._deadState);
            return;
        }

        if (ownerMannequin is not null)
        {
            if (!ownerMannequin.isEngaging) return;
        }

        attackTimer -= Time.deltaTime;

        //rotate towards player to attempt to land an attack
        Vector3 dir = Owner.Player.transform.position - Owner.transform.position;
        dir.y = 0;
        Quaternion rot = Quaternion.LookRotation(dir);
        // slerp to the desired rotation over time
        Owner.transform.rotation = Quaternion.Slerp(Owner.transform.rotation, rot, 5f * Time.deltaTime);
        
        // Continue moving towards the player if they can move while attacking
        if ((Owner as IHasMeleeAttack).moveWhileAttacking)
                if (Owner.agent.isActiveAndEnabled && Owner.agent.isOnNavMesh) 
                    Owner.agent.SetDestination(Owner.Player.transform.position);

        if (attackTimer <= 0)
        {
            //if player is still within attack range after the animation finished playing, player takes damage
            //TO REVISE: if the enemy doesn't do damage to the player have them turn instead of keep attacking the air
            if ((Owner as IHasMeleeAttack).PlayerInTrigger && !hitPlayer)
            {
                Owner.Player.TakeDamage(Owner.damage * Enemy.DamageMultiplier);
                hitPlayer = true;
            }

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
    }
}
