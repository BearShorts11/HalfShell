using UnityEngine;

/// <summary>
/// Handles enemy chase behavior: state where enemy will close distance between itself and the player until it is within attacking distance
/// </summary>
public class ChaseState : State
{
    //public IEnemy Owner { get; private set; }

    private MannequinEnemy OwnerMannequin;
    float attackRange;
    Vector3 destination;

    public ChaseState(Enemy owner)
    {
        Owner = owner;
        this.attackRange = owner.attackRange;
        OwnerMannequin = Owner as MannequinEnemy;
    }

    public override void Enter()
    {
        this.attackRange = Owner.attackRange;
        if(Owner.agent.isOnNavMesh) Owner.agent.isStopped = false;

        Owner.SpottedPlayer();

        //have owner as jugg set ranged attack time
        if (Owner is Juggernaut) (Owner as Juggernaut).SetNextRangedAttackTime();

        if (Owner is MannequinEnemy)
        {
            if (OwnerMannequin.goal == MannequinEnemy.Goal.GetWeapon)
            {
                if (!OwnerMannequin.nearWeapon)
                    destination = OwnerMannequin.FindNearestWeapon().transform.position;
            }
        }
    }

    public override void Exit()
    {
        
    }

    public override void Update()
    {
        destination = Owner.Player.transform.position;

        if (OwnerMannequin && OwnerMannequin.goal == MannequinEnemy.Goal.GetWeapon)
        {
            if (destination != OwnerMannequin.nearWeapon.transform.position)
            {
                destination = OwnerMannequin.nearWeapon.transform.position;
            }
            if ((Owner.transform.position - destination).magnitude <= Mathf.Pow(OwnerMannequin.nearWeapon.pickupRange, 2))
            {
                OwnerMannequin.GrabWeapon(OwnerMannequin.nearWeapon);
            }
        }
        float distanceFromPlayer = Vector3.Distance(Owner.transform.position, Owner.Player.transform.position);
        if (distanceFromPlayer <= attackRange)
        {
            //differnt chase behaviors based on what subtype of enemy is chasing the player
            switch (Owner)
            {
                case RangedEnemy:
                    Owner.stateMachine.TransitionTo(Owner.stateMachine._shootState);
                    break;
                case MannequinEnemy:
                    if ((Owner as MannequinEnemy).isEngaging)
                        Owner.stateMachine.TransitionTo(Owner.stateMachine._meleeAttackState);
                    break;
                default:
                    if (Owner.agent.isOnNavMesh) Owner.agent.isStopped = true;
                    Owner.stateMachine.TransitionTo(Owner.stateMachine._meleeAttackState);
                    //if ((Owner as IHasMeleeAttack).PlayerInTrigger) Owner.stateMachine.TransitionTo(Owner.stateMachine._meleeAttackState);
                    //else if (Owner.agent.isActiveAndEnabled && Owner.agent.isOnNavMesh) Owner.agent.SetDestination(Owner.Player.transform.position);
                    break;
            }
        }
        else if ((Owner is RangedEnemy || Owner is MannequinEnemy) && distanceFromPlayer > Owner.detectionRange && !Owner.AlwaysChase)
        {
            Owner.stateMachine.TransitionTo(Owner.stateMachine._idleState);
        }
        else if ((Owner is Juggernaut) && (Owner as Juggernaut).nextTimeToRangedAttack <= Time.time)
        {
            Owner.stateMachine.TransitionTo(Owner.stateMachine._shootState);
        }
        else
        {
            //attempting to avoid editor errors
            if (Owner.agent.isActiveAndEnabled && Owner.agent.isOnNavMesh) Owner.agent.SetDestination(destination);
        }
    }
}
