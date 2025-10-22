using UnityEngine;

public class HalfShell : ShellBase
{
    public HalfShell()
    {
        Size = 0.5f;
        Damage = 6;
        AmtProjectiles = 9;
        MaxRange = 25f;
        type = ShellType.HalfShell;
        DisplayColor = Color.red;
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
            case <= 5f:
                damageModifier *= 0.1f;
                break;
            case > 5f and <= 10f:
                damageModifier *= 0.05f;
                break;
            case > 10f:
                damageModifier = 0;
                break;
        }

        return Damage + damageModifier;
    }
    
}
