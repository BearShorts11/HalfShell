using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DocileState : State
{
    public DocileState(Enemy owner)
    { 
        this.Owner = owner;
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        //do nothing. Yup. 
    }
}
