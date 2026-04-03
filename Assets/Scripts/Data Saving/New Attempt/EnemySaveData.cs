using System;
using System.Collections.Generic;

public class EnemySaveData : ObjectSaveData
{
    private Enemy enemy;
    public float Health { get; private set; }
    public State State { get; private set; }

    private void Start()
    {
        enemy = GetComponent<Enemy>();
    }

    public override void OnSave()
    {
        base.OnSave();

        this.Health = enemy.Health;
        this.State = enemy.stateMachine.CurrentState;
    }

    public override void OnLoad()
    {
        if (!hasBeenSaved) return;

        base.OnLoad();

        if (enemy.Health <= 0 && this.Health > 0) enemy.Revive();
        enemy.SetHealth(this.Health);

        if (enemy.stateMachine.CurrentState == enemy.stateMachine._deadState && this.State != enemy.stateMachine._deadState) enemy.Revive();
        enemy.stateMachine.TransitionTo(this.State);
    }

}