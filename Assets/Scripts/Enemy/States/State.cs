using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public abstract class State
{
    /// <summary>
    /// Which enemy the states belong to/apply their behavior to.
    /// The Owner has a state machine which has an instance of each of the states. 
    /// However, the states need to reference the owner in order to perform their behaviors (ex. getting variables for damage)
    /// </summary>
    public Enemy Owner { get; protected set; }

    /// <summary>
    /// Behavior on entering a state
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// Handles behavior of the state conditionally based on the type of Owner. Handles behavior and transitions between state. Called from a StateMachine's Update()
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Behavior on exiting a state
    /// </summary>
    public abstract void Exit();

}
