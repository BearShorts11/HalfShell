using UnityEngine;
    
public class Incindiary : ShellBase
{
    public static int MaxHolding;

    public Incindiary()
    {
        Size = 1f;
        Damage = 15;
        AmtProjectiles = 20;
        MaxRange = 30f;
        SpreadRange = .25f;
        type = ShellType.Incindiary;
        DisplayColor = new Color(255, 155, 40);
        MaxHolding = 5;

        hasSpecialEffects = true;
        effect = HitEffect.Fire;

        effectTime = 5;
        effectHitPerSecond = 1;
        effectDamage = 20;
    }

    public override float ScaleDamage(RaycastHit hit)
    {
        return Damage;
    }
}
