using UnityEngine;

public class ShellBase : MonoBehaviour
{
    public int AmtProjectiles;
    public int Size;

    public float Damage;

    public bool hasSpecialEffects;
    //something to store effects (enum??)

    //move outside the class??
    public enum ShellType
    { 
        Buckshot,
        Slug
    }

    protected ShellType type;
    public ShellType Type { get { return type; } }


}
