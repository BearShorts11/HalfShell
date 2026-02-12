using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IState
{
    public WIPEnemy Owner { get; set; }

    public void Enter();

    public void Update();

    public void Exit();

}
