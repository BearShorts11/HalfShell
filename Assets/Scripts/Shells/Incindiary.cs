using UnityEngine;
    
public class Incindiary : ShellBase
{
    public Incindiary()
    {
        Size = 1f;
        Damage = 0;
        AmtProjectiles = 9;
        MaxRange = 100f;
        type = ShellType.Incindiary;
        DisplayColor = new Color(255, 155, 40);
        MaxHolding = 5;

        hasSpecialEffects = true;
        effect = HitEffect.Fire;

        effectTime = 3;
        effectHitPerSecond = 1;
        effectDamage = 10;
    }

    public override float ScaleDamage(RaycastHit hit)
    {
        return Damage;
    }
}
