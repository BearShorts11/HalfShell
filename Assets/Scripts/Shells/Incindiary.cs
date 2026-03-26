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
        DisplayColor = new Color(255, 155, 40);
        MaxHolding = 5;

        hasSpecialEffects = true;
        effect = HitEffect.Fire;

        effectTime = 5;
        effectHitPerSecond = 1;
        effectDamage = 3;
    }

    public override float ScaleDamage(RaycastHit hit)
    {
        return Damage;
    }
}
