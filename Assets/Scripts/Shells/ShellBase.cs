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
        Buckshot,
        Slug,
        HalfShell,
        BeanBag,
        BMG
    }

    protected ShellType type;
    public ShellType Type { get { return type; } }

    /// <summary>
    /// scales the damage based on how far a shell was shot
    /// </summary>
    /// <param name="hit"> raycast hit to determine distance </param>
    /// <returns> scaled damage float </returns>
    public abstract float ScaleDamage(RaycastHit hit);

}
