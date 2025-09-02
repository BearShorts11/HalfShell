using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour
{
    //AKA Gun class
    //https://www.youtube.com/watch?v=THnivyG0Mvo 

    public Camera fpsCam;

    private float reloadTime = 1f; //time to load one shell
    private float nextTimeTofire = 0f;
    private int totalCapacity = 5;
    [SerializeField] private int currentCapacity = 0; //shown for debug purposes
    private float shotCooldown = 1f; //time in between shots
    [SerializeField] private float spreadRange = 0.1f; //variation in raycasts for non single shots (random spread)
    private float gunRange = 100f;

    //UI fields
    public TextMeshProUGUI spaceLeftText;
    public Image chamberUI;
    public Image SingleShotCrosshair;
    public Image MultiShotCrosshair;


    //first in last out collection
    private Stack<ShellBase> chamber = new Stack<ShellBase>();

    private bool isInMenu = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";

        isInMenu = false;
        //spaceLeftText.gameObject.SetActive(false);

        SingleShotCrosshair.gameObject.SetActive(true);
        MultiShotCrosshair.gameObject.SetActive(false);
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

        if (chamber.Count > 0)
        { 
            ShellBase top = chamber.Peek();
            switch (top.Type)
            { 
                case ShellBase.ShellType.Slug:
                //case ShellBase.ShellType. some other shot type that also is a single fire
                    SingleShotCrosshair.gameObject.SetActive(true);
                    MultiShotCrosshair.gameObject.SetActive(false);
                    break;
                case ShellBase.ShellType.Buckshot:
                    SingleShotCrosshair.gameObject.SetActive(false);
                    MultiShotCrosshair.gameObject.SetActive(true);
                    break;
            }
        }

        //not being used rn
        //if (Input.GetKeyDown(KeyCode.Q) && isInMenu == false)
        //{ 
        //    isInMenu = true;
        //    spaceLeftText.gameObject.SetActive(true);
        //    DoneButton.SetActive(true);
        //    BuckButton.SetActive(true);
        //    SlugButton.SetActive(true);

        //    gameObject.GetComponent<PlayerBehavior>().NoMove();
        //}
        //if (isInMenu == true && Input.GetKeyDown(KeyCode.Q))
        //{
        //    isInMenu = false;
        //    spaceLeftText.gameObject.SetActive(false);
        //    DoneButton.SetActive(false);
        //    BuckButton.SetActive(false);
        //    SlugButton.SetActive(false);

        //    gameObject.GetComponent<PlayerBehavior>().YesMove();
        //}

        if (Input.GetKeyDown(KeyCode.X)) AddSlug();
        if (Input.GetKeyDown(KeyCode.C)) AddBuckshot();


    }



    //change to coroutine to do cooldown time?? why yes I just don't want to do that rn
    public void LoadChamber(ShellBase shell)
    {
        if (currentCapacity + shell.Size <= totalCapacity)
        { 
            chamber.Push(shell);
            //currentCapacity += shell.Size;
            currentCapacity++;

            spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";
        }

    }

    public void AddSlug()
    {
        Slug slug = new Slug();
        if (currentCapacity + slug.Size <= totalCapacity)
        { 
            LoadChamber(slug);
            Debug.Log("slug pressed");

            //this is messy I know I'll fix it later
            chamberUI.transform.GetChild(currentCapacity - 1).gameObject.SetActive(true);
            chamberUI.transform.GetChild(currentCapacity - 1).gameObject.GetComponent<Image>().color = Color.green;
            
        }
    }

    public void AddBuckshot()
    { 
        Buckshot buck = new Buckshot();
        if (currentCapacity + buck.Size <= totalCapacity)
        { 
            LoadChamber(buck);
            Debug.Log("buck pressed");

            chamberUI.transform.GetChild(currentCapacity - 1).gameObject.SetActive(true);
            chamberUI.transform.GetChild(currentCapacity - 1).gameObject.GetComponent<Image>().color = Color.red;
            
        }
    }



    public void Fire()
    {
        if (currentCapacity > 0)
        {
            ShellBase shell = chamber.Pop();
            //currentCapacity -= shell.Size;
            currentCapacity--;
            chamberUI.transform.GetChild(currentCapacity).gameObject.SetActive(false);
            spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";

            //determine behavior of shot based on shell type

            RaycastHit hit;
            switch (shell.Type)
            {
                case ShellBase.ShellType.Buckshot:

                    for (int i = 1; i <= shell.AmtProjectiles; i++)
                    {
                        //https://discussions.unity.com/t/raycast-bullet-spread/753464 
                        Vector3 fwd = fpsCam.transform.forward;
                        fwd += fpsCam.transform.TransformDirection(new Vector3(Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange)));
                        if (Physics.Raycast(fpsCam.transform.position, fwd, out hit, gunRange))
                        {
                            Debug.DrawLine(fpsCam.transform.position, hit.point, Color.red, 5f);
                            HitEnemy(hit,shell);
                        }
                    
                    }

                    break;
                case ShellBase.ShellType.Slug:
                    if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunRange))
                    {
                        HitEnemy(hit, shell);
                    }
                    break;
            }



        }
        else Debug.Log("cannot fire");

    }

    private void HitEnemy(RaycastHit hit, ShellBase shell)
    {
        Debug.Log(hit.transform.name);

        EnemyBehavior enemy = hit.transform.GetComponent<EnemyBehavior>();

        if (enemy != null)
        {
            enemy.Damage(shell.Damage); //eventually will be shell.Damage instead of a random number;
            Debug.Log("enemy hit");
        }
    }
}
