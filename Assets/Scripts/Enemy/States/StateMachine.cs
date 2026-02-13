using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class StateMachine
{
    //From: https://learn.unity.com/tutorial/develop-a-modular-flexible-codebase-with-the-state-programming-pattern
    //And: https://gameprogrammingpatterns.com/state.html 

    public IState CurrentState { get; private set; }

    public IdleState _idleState;
    public ChaseState _chaseState;
    public MeleeAttackState _meleeAttackState;
    public DeadState _deadState;
    public CooldownState _cooldownState;
    
    public event Action<IState> stateChanged;


    private IEnemy Owner;

    public StateMachine(IEnemy owner)
    { 
        Owner = owner;

        _meleeAttackState = new MeleeAttackState(owner);
        _cooldownState = new CooldownState(owner);
        _chaseState = new ChaseState(owner);
        _idleState = new IdleState(owner);
        _deadState = new DeadState(owner);
    }

    /// <summary>
    /// Set the starting state. ONLY pass THIS state machine's state fields as parameters
    /// </summary>
    /// <param name="state"></param>
    public void Initialize(IState state)
    {
        CurrentState = state;
        state.Enter();


        // notify other objects that state has changed
        stateChanged?.Invoke(state);
    }


    /// <summary>
    /// Exit current state and enter another. ONLY pass THIS state machine's state fields as parameters
    /// </summary>
    /// <param name="nextState"></param>
    public void TransitionTo(IState nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();


        // notify other objects that state has changed
        stateChanged?.Invoke(nextState);
    }


    // allow the StateMachine to update this state
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }

}
