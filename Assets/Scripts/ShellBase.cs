using UnityEngine;

public class ShellBase : MonoBehaviour
{
    public int AmtProjectiles;
    public int Size;

    public float Damage;

    public bool hasSpecialEffects;
    //something to store effects (enum??)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Size = 1;
        Damage = 10;
        AmtProjectiles = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
