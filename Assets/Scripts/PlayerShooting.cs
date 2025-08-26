using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    //AKA Gun class

    public Camera fpsCam;

    private float spreadAmount;
    private float reloadTime = 1f; //time to load one shell
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
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("pressed L mouse button");
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
        //if (chamber.Count > 0)
        //{ 
            //ShellBase shell = chamber.Pop();
            //currentCapacity -= shell.Size;
            //determine behavior of shot based on shell type

            RaycastHit hit;
            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunRange))
            {
                Debug.Log(hit.transform.name);
            }
        
        //}

    }
}
