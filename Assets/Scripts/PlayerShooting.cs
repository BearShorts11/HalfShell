using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    //public Image chamberUI;
    public TextMeshProUGUI spaceLeftText;
    public GameObject DoneButton; //I don't need this but I'm too lazy to delete it rn -N
    public GameObject BuckButton;
    public GameObject SlugButton;

    //first in last out collection
    private Stack<ShellBase> chamber;

    private bool isInMenu = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";

        isInMenu = false;
        spaceLeftText.gameObject.SetActive(false);
        DoneButton.SetActive(false);
        BuckButton.SetActive(false);
        SlugButton.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //both L mouse & L control can fire as per the system currently being used. just thought I would note, can fix/change later
        if (Input.GetButton("Fire1") && Time.time > nextTimeTofire && isInMenu == false)
        {
            Debug.Log("pressed L mouse button");
            nextTimeTofire = Time.time + 1 / reloadTime;
            Fire();
        }

        if (Input.GetKeyDown(KeyCode.Q) && isInMenu == false)
        { 
            isInMenu = true;
            spaceLeftText.gameObject.SetActive(true);
            DoneButton.SetActive(true);
            BuckButton.SetActive(true);
            SlugButton.SetActive(true);
        }
        if (isInMenu == true && Input.GetKeyDown(KeyCode.Escape))
        {
            isInMenu = false;
            spaceLeftText.gameObject.SetActive(false);
            DoneButton.SetActive(false);
            BuckButton.SetActive(false);
            SlugButton.SetActive(false);
        }
    }



    //change to coroutine to do cooldown time?? why yes I just don't want to do that rn
    public void LoadChamber(ShellBase shell)
    {
        if (currentCapacity + shell.Size <= totalCapacity)
        { 
            chamber.Push(shell);
            currentCapacity += shell.Size;

            spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";
        }

    }

    public void AddSlug()
    { 
        Slug slug = new Slug();
        LoadChamber (slug);
    }

    public void AddBuckshot()
    { 
        Buckshot buck = new Buckshot();
        LoadChamber(buck);
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
