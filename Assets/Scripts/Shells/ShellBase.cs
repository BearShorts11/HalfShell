using UnityEngine;

public abstract class ShellBase : MonoBehaviour
{
    public int AmtProjectiles;
    public float Size;
    public float Damage;
    [SerializeField] public float MaxRange;
    public int MaxHolding;

    public bool hasSpecialEffects;
    //something to store effects (enum??)

    public Color DisplayColor;

    public enum ShellType
    { 
        HalfShell = 1,
        Slug = 2,
        Buckshot = 3,
        BeanBag = 4,
        BMG = 5,
        Incindiary = 6
    }

    /// <summary>
    /// own class???
    /// </summary>
    public enum HitEffect
    { 
        Stun = 0,
        Fire = 1
    }

    protected ShellType type;
    public ShellType Type { get { return type; } }

    protected HitEffect effect;
    public HitEffect Effect { get { return effect; } }
    public float effectTime { get; protected set; }
    public float effectHitPerSecond { get; protected set; }

    public float effectDamage { get; protected set; }

    /// <summary>
    /// scales the damage based on how far a shell was shot
    /// </summary>
    /// <param name="hit"> raycast hit to determine distance </param>
    /// <returns> scaled damage float </returns>
    public abstract float ScaleDamage(RaycastHit hit);

}
