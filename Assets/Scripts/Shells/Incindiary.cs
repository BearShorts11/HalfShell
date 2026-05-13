using UnityEngine;
    
public class Incindiary : ShellBase
{
    public static int MaxHolding;

    public Incindiary()
    {
        Size = 1f;
        Damage = 3;
        AmtProjectiles = 5;
        MaxRange = 100f;
        SpreadRange = 0.2f;
        type = ShellType.Incindiary;
        DisplayColor = new Color(255, 155, 40);
        MaxHolding = 5;

        hasSpecialEffects = true;
        effect = HitEffect.Fire;

        effectTime = 5;
        effectHitPerSecond = 1;
        effectDamage = 15;
    }

    public override float ScaleDamage(RaycastHit hit)
    {
        return Damage;
    }
}
