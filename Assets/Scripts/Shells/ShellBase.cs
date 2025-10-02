using UnityEngine;

public class ShellBase : MonoBehaviour
{
    public int AmtProjectiles;
    public float Size;

    public float Damage;

    public bool hasSpecialEffects;
    //something to store effects (enum??)

    //move outside the class??
    public enum ShellType
    { 
        Buckshot,
        Slug,
        HalfShell
    }

    protected ShellType type;
    public ShellType Type { get { return type; } }


}
