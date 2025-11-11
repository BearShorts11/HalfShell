using UnityEngine;
using static ShellBase;

public class BMG : ShellBase
{
    public BMG()
    {
        Size = 2;
        Damage = 120;
        AmtProjectiles = 1;
        MaxRange = 200f;
        type = ShellType.BMG;
        DisplayColor = Color.yellow;
        MaxHolding = 3; //2??
    }

    public override float ScaleDamage(RaycastHit hit)
    {
        return Damage;
    }
}
