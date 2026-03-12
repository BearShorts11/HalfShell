using UnityEngine;
    
public class Incindiary : ShellBase
{
    public Incindiary()
    {
        Size = 1f;
        Damage = 6;
        AmtProjectiles = 12;
        MaxRange = 200f;
        type = ShellType.Incindiary;
        DisplayColor = Color.yellow;
        MaxHolding = 5;

        hasSpecialEffects = true;
    }

    public override float ScaleDamage(RaycastHit hit)
    {
        return Damage;
    }
}
