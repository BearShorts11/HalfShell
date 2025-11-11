using UnityEngine;

public class BeanBag : ShellBase
{
    public BeanBag()
    {
        Size = 1;
        Damage = 25f;
        AmtProjectiles = 1;
        MaxRange = 50;
        type = ShellType.BeanBag;
        DisplayColor = Color.yellow;
        MaxHolding = 10;

        hasSpecialEffects = true;
    }
    public override float ScaleDamage(RaycastHit hit)
    { 
        return Damage;
    }

    public void Stun()
    { 
        
    }
}
