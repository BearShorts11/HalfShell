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

    #region Gun Stats
    //Add [SerializeField] in front of anything that needs tweaking/balancing
    private float reloadTime = 1f; //time to load one shell
    private float nextTimeTofire = 0f;

    private int totalCapacity = 5;
    //change to float when half shells get implimented, do away/change UI by then (breaks referencing)
    [SerializeField] private int currentCapacity = 0; //shown for debug purposes

    private float shotCooldown = 1f; //time in between shots
    private float nextTimeToShot = 0f;
    [SerializeField] private float spreadRange = 0.1f; //variation in raycasts for non single shots (random spread)
    private float gunRange = 100f;
    #endregion

    #region UI fields
    //UI fields
    public TextMeshProUGUI spaceLeftText;
    public Image chamberUI;
    public Image SingleShotCrosshair;
    public Image MultiShotCrosshair;
    public GameObject ShellSelectionMenu;
    #endregion


    //first in last out collection
    private Stack<ShellBase> magazine = new Stack<ShellBase>();
    private ShellBase chamber;

    //added so the player doesn't negligently discharge while interacting with UI -A
    public static bool canFire = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";

        SingleShotCrosshair.gameObject.SetActive(true);
        MultiShotCrosshair.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //both L mouse & L control can fire as per the system currently being used. just thought I would note, can fix/change later
        if (canFire && Input.GetButton("Fire1") && Time.time > nextTimeTofire)
        {
            Debug.Log("pressed L mouse button");
            nextTimeTofire = Time.time + 1 / reloadTime;
            Fire();
        }

        if (chamber != null)
        {
            ShellBase top = chamber;
            SwitchCrosshairUI(top);
        }
        else
        {
            //reset to small crosshair (for visibility)
            SingleShotCrosshair.gameObject.SetActive(true);
            MultiShotCrosshair.gameObject.SetActive(false);
        }
        

        if (Input.GetKeyDown(KeyCode.Keypad1) | Input.GetKeyDown(KeyCode.Alpha1)) AddBuckshot();
        if (Input.GetKeyDown(KeyCode.Keypad2) | Input.GetKeyDown(KeyCode.Alpha2)) AddSlug();


        //racking back
        if (Input.GetKeyDown(KeyCode.Mouse1))
        { 
            canFire = false;
            Debug.Log("racked back");
            chamber = null;
            //eject chamber
        }
        //racking forwards
        if (Input.GetKeyUp(KeyCode.Mouse1))
        { 
            canFire = true;
            Debug.Log("racked forwards");
            if (magazine.Count > 0) chamber = magazine.Pop();
        }

        //Opens Shell Selection menu UI while [TAB] is pressed - Alex
        //if (Input.GetKey(KeyCode.Tab)) ShellSelectionButton.OpenShellWheel(ShellSelectionMenu);

    }

    //private void OpenShellWheel()
    //{
    //    Debug.Log("opened shell selection menu");
    //    isInMenu = true;
    //}


    private void SwitchCrosshairUI(ShellBase top)
    {
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

    
    public void LoadMag(ShellBase shell)
    {
        if (currentCapacity + shell.Size <= totalCapacity)
        { 
            //start anim
            magazine.Push(shell);
            int size = shell.Size;
            currentCapacity += size;

            spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";
        }

    }

    public void AddSlug()
    {
        Slug slug = new Slug();
        if (currentCapacity + slug.Size <= totalCapacity)
        { 
            LoadMag(slug);
            Debug.Log("slug pressed");

            //move to LoadChamber() dependant on how we want to display what's in the chamber, if at all 
            GameObject display = chamberUI.transform.GetChild(currentCapacity - 1).gameObject;
            display.SetActive(true);
            display.GetComponent<Image>().color = Color.green;
            
        }
    }

    public void AddBuckshot()
    { 
        Buckshot buck = new Buckshot();
        if (currentCapacity + buck.Size <= totalCapacity)
        { 
            LoadMag(buck);
            Debug.Log("buck pressed");

            GameObject display = chamberUI.transform.GetChild(currentCapacity - 1).gameObject;
            display.SetActive(true);
            display.GetComponent<Image>().color = Color.red;
        }
    }



    public void Fire()
    {
        if (chamber != null && canFire)
        {
            ShellBase shell = magazine.Pop();
            int size = shell.Size;
            currentCapacity -= size;
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

            canFire = false;
            //shell fired but needs to be ejected from chamber

        }
        else Debug.Log("cannot fire");

    }

    private void HitEnemy(RaycastHit hit, ShellBase shell)
    {
        Debug.Log(hit.transform.name);

        EnemyBehavior enemy = hit.transform.GetComponent<EnemyBehavior>();

        if (enemy != null)
        {
            enemy.Damage(shell.Damage); 
            Debug.Log("enemy hit");
        }
    }
}
