using UnityEngine;

public class Slug : ShellBase
{
    public Slug()
    {
        Size = 1;
        Damage = 80;
        AmtProjectiles = 1;
        MaxRange = 200f;
        type = ShellType.Slug;
        DisplayColor = Color.green;
        MaxHolding = 20; //eventually 15
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override float ScaleDamage(RaycastHit hit)
    {
        if (hit.distance > MaxRange) return 0; //just in case
        float damageModifier = Damage;

        switch (hit.distance)
        {
            case > 50f and <= 100f:
                damageModifier *= -0.05f;
                break;
            case > 100f:
                damageModifier *= -0.1f;
                break;
            default:
                damageModifier = 0;
                break;
        }

        return Damage + damageModifier;
    }
}
