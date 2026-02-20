using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles logic for switching enemy states
/// </summary>
public class StateMachine
{
    //From: https://learn.unity.com/tutorial/develop-a-modular-flexible-codebase-with-the-state-programming-pattern
    //And: https://gameprogrammingpatterns.com/state.html 

    /// <summary>
    /// The current state of the state machine
    /// </summary>
    public State CurrentState { get; private set; }

    //instances of each possible state
    public IdleState _idleState;
    public ChaseState _chaseState;
    public MeleeAttackState _meleeAttackState;
    public ShootState _shootState;
    public DeadState _deadState;
    public CooldownState _cooldownState;
    public DocileState _docileState;

    private Enemy Owner;

    public StateMachine(Enemy owner)
    { 
        Owner = owner;

        _meleeAttackState = new MeleeAttackState(owner);
        _shootState = new ShootState(owner);
        _cooldownState = new CooldownState(owner);
        _chaseState = new ChaseState(owner);
        _idleState = new IdleState(owner);
        _deadState = new DeadState(owner);
        _docileState = new DocileState(owner);
    }

    /// <summary>
    /// Set the starting state. ONLY pass THIS state machine's state fields as parameters
    /// </summary>
    /// <param name="state"></param>
    public void Initialize(State state)
    {
        CurrentState = state;
        state.Enter();
    }

    /// <summary>
    /// Exit current state and enter another. ONLY pass THIS state machine's state fields as parameters
    /// </summary>
    /// <param name="nextState"></param>
    public void TransitionTo(State nextState)
    {
        CurrentState.Exit();
        CurrentState = nextState;
        nextState.Enter();
    }

    /// <summary>
    /// Allows state machine to update the current state
    /// </summary>
    public void Update()
    {
        if (CurrentState != null)
        {
            CurrentState.Update();
        }
    }

}
