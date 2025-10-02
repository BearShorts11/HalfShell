using UnityEngine;

public class ShellBase : MonoBehaviour
{
    public int AmtProjectiles;
    public float Size;

    public float Damage;

    public bool hasSpecialEffects;
    //something to store effects (enum??)

    public Color DisplayColor;

    public enum ShellType
    { 
        Buckshot,
        Slug,
        HalfShell
    }

    protected ShellType type;
    public ShellType Type { get { return type; } }


}
