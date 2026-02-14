using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IState
{
    public IEnemy Owner { get; set; }

    /// <summary>
    /// Behavior on entering a state
    /// </summary>
    public void Enter();

    /// <summary>
    /// Handles behavior of the state conditionally based on the type of Owner. Handles behavior and transitions between state
    /// </summary>
    public void Update();

    /// <summary>
    /// Behavior on exiting a state
    /// </summary>
    public void Exit();

}
