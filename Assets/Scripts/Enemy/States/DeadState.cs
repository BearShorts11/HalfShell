using UnityEngine;
using UnityEditor;

public class DeadState : IState
{
    public IEnemy Owner { get; set; }

    public DeadState(IEnemy owner)
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

        //handle death. comment out below line when ragdolling gets added back
        Object.Destroy(Owner.gameObject);
        //call methods from owner to do all the things
        
    }
}
