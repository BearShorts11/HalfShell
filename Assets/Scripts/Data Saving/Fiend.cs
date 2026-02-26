using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Fiend : MonoBehaviour, IBind<EnemyData>
{
    [SerializeField] private SerializableGuid _id = new SerializableGuid(Guid.NewGuid());
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [SerializeField] EnemyData data;
    [SerializeField] Enemy fiend;

    public void Bind(EnemyData data)
    {
        this.data = data;
        this.data.Id = Id;
        transform.position = data.position;
        fiend.stateMachine.TransitionTo(ConvertStringToState(data.State));
    }

    private void Update()
    {
        data.position = transform.position;
        data.Health = fiend.Health;
        data.State = ConvertStateToString();
    }

    private string ConvertStateToString()
    {
        switch (fiend.stateMachine.CurrentState)
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
        switch (state)
        {
            case "ChaseState":
                return fiend.stateMachine._chaseState;
            case "CooldownState":
                return fiend.stateMachine._cooldownState;
            case "DeadState":
                return fiend.stateMachine._deadState;
            case "DocileState":
                return fiend.stateMachine._docileState;
            case "MeleeAttackState":
                return fiend.stateMachine._meleeAttackState;
            case "ShootState":
                return fiend.stateMachine._shootState;
            default:
                return fiend.stateMachine._idleState;
        }
    }
}
