using Assets.Scripts;
using System;
using UnityEngine;

public class Fiend : MonoBehaviour, IBind<EnemyData>
{
    [SerializeField] private SerializableGuid _id = new SerializableGuid(Guid.NewGuid());
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [SerializeField] public EnemyData data;
    [SerializeField] Enemy enemy;

    public void Bind(EnemyData data)
    {
        this.data = data;
        this.data.Id = Id;

        //only place in position indicated when reloading
        if (this.data.FirstBind) this.data.FirstBind = false;
        else
        { 
            //only update values on reloading so how they're placed in the scene stays the same 
            transform.position = data.position;

            enemy.SetHealth(data.Health);

            if (enemy.stateMachine is not null)
            {
                //if was dead and on loading will not be dead "revive" it
                if (enemy.stateMachine.CurrentState == enemy.stateMachine._deadState &&
                    ConvertStringToState(data.State) != enemy.stateMachine._deadState)
                {
                    enemy.Revive();
                }
                enemy.stateMachine.TransitionTo(ConvertStringToState(data.State));
            }
            //if (enemy.stateMachine.CurrentState == enemy.stateMachine._deadState) Destroy(this.gameObject);
        }

    }

    private void Update()
    {
        data.position = transform.position;
        data.Health = enemy.Health;
        data.State = ConvertStateToString();
    }

    private string ConvertStateToString()
    {
        switch (enemy.stateMachine.CurrentState)
        {
            case ChaseState:
                return "ChaseState";
            case CooldownState:
                return "CooldownState";
            case DeadState:
                return "DeadState";
            case DocileState:
                return "DocileState";
            case MeleeAttackState:
                return "MeleeAttackState";
            case ShootState:
                return "ShootState";
            default:
                return "IdleState";

        }
    }

    private State ConvertStringToState(string state)
    {
        if (enemy is null) return null;
        if (enemy.stateMachine is null) return null;

        switch (state)
        {
            case "ChaseState":
                return enemy.stateMachine._chaseState;
            case "CooldownState":
                return enemy.stateMachine._cooldownState;
            case "DeadState":
                return enemy.stateMachine._deadState;
            case "DocileState":
                return enemy.stateMachine._docileState;
            case "MeleeAttackState":
                return enemy.stateMachine._meleeAttackState;
            case "ShootState":
                return enemy.stateMachine._shootState;
            default:
                return enemy.stateMachine._idleState;
        }
    }
}
