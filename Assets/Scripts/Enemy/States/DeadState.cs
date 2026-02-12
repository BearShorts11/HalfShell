using UnityEngine;
using UnityEditor;

public class DeadState : IState
{
    public WIPEnemy Owner { get; set; }

    public DeadState(WIPEnemy owner)
    { 
        this.Owner = owner;
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        Debug.Log("Dead State");

        Owner.agent.isStopped = true;

        //handle death. 
        Object.Destroy(Owner.gameObject);
        //call methods from owner to do all the things
        
    }
}
