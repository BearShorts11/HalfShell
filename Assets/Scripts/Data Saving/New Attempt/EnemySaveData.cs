using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemySaveData : ObjectSaveData
{
    public Enemy Enemy { get; private set; }
    public float Health { get; private set; }
    public State State { get; private set; }

    private void Start()
    {
        Enemy = GetComponent<Enemy>();
    }

    public override void OnSave()
    {
        base.OnSave();

        this.Health = Enemy.Health;
        this.State = Enemy.stateMachine.CurrentState;
    }

    public override void OnLoad()
    {
        if (!hasBeenSaved) return;

        base.OnLoad();

        if (Enemy.Health <= 0 && this.Health > 0) Enemy.Revive();
        Enemy.SetHealth(this.Health);

        if (Enemy.stateMachine.CurrentState == Enemy.stateMachine._deadState && this.State != Enemy.stateMachine._deadState) Enemy.Revive();
        Enemy.stateMachine.TransitionTo(this.State);
    }

}