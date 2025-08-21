using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    //AKA Gun class

    [SerializeField] private float spreadAmount;

    private float reloadTime = 1f; //time to load one shell
    private int capacity = 5;
    private float shotCooldown = 1f; //time in between shots
    private float spreadRange = 3f; //variation in raycasts for non single shots (random spread)

    //array or list? keep as array for now but if shells take up half a chamber space prob needs to be a list?
    public int[] chamber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
