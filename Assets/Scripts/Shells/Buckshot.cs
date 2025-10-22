using UnityEngine;

public class Buckshot : ShellBase
{
    public Buckshot()
    {
        Size = 1;
        Damage = 12;
        AmtProjectiles = 9;
        MaxRange = 100f;
        type = ShellType.Buckshot; 
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
        throw new System.NotImplementedException();
    }
}
