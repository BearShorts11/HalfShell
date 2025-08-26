using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    //AKA Gun class
    //https://www.youtube.com/watch?v=THnivyG0Mvo 

    public Camera fpsCam;

    private float spreadAmount;
    private float reloadTime = 1f; //time to load one shell
    private float nextTimeTofire = 0f;
    private int totalCapacity = 5;
    private int currentCapacity = 0;
    private float shotCooldown = 1f; //time in between shots
    private float spreadRange = 3f; //variation in raycasts for non single shots (random spread)
    private float gunRange = 100f; 


    //first in last out collection
    private Stack<ShellBase> chamber;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1") && Time.time > nextTimeTofire)
        {
            Debug.Log("pressed L mouse button");
            nextTimeTofire = Time.time + 1 / reloadTime;
            Fire();
        }
    }

    //change to coroutine to do cooldown time?? why yes I just don't want to do that rn
    public void LoadChamber(ShellBase shell)
    {
        if (currentCapacity + shell.Size <= totalCapacity)
        { 
            chamber.Push(shell);
            currentCapacity += shell.Size;
        }

    }

    public void Fire()
    {
        //got overzealous and put in this code before it works -N
        //if (chamber.Count > 0)
        //{ 
            //ShellBase shell = chamber.Pop();
            //currentCapacity -= shell.Size;
            //determine behavior of shot based on shell type

            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunRange))
            {
                Debug.Log(hit.transform.name);

            EnemyBehavior enemy = hit.transform.GetComponent<EnemyBehavior>();

            if (enemy != null)
            {
                enemy.Damage(10f); //eventually will be shell.Damage instead of a random number;
                Debug.Log("enemy hit");
            }
                //hit.transform.GetComponent<Target>
            }
        
        //}

    }
}
