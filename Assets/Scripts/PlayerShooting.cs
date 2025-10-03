using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;

public class PlayerShooting : MonoBehaviour
{
    //AKA Gun class
    //https://www.youtube.com/watch?v=THnivyG0Mvo 

    public Camera fpsCam;

    [SerializeField] private ParticleSystem blood;
    [SerializeField] private ParticleSystem dust;

    public Animator animator;

    //Add [SerializeField] in front of anything that needs tweaking/balancing

    //private float reloadTime = 1f; //time to load one shell
    //private float nextTimeTofire = 0f;

    private float totalCapacity = 5;
    //change to float when half shells get implimented, do away/change UI by then (breaks referencing)
    [SerializeField] private float currentCapacity = 0; //shown for debug purposes
    private float shotCooldown = 1f; //time in between shots
    private float nextTimeToShot = 0f;
    [SerializeField] private float spreadRange = 0.1f; //variation in raycasts for non single shots (random spread)
    private float gunRange = 100f;

    #region UI fields - move to own object
    //UI fields
    public TextMeshProUGUI spaceLeftText;
    public Image magazineUI;
    public Image chamberShell;
    public Image SingleShotCrosshair;
    public Image MultiShotCrosshair;
    public GameObject ShellSelectionMenu;
    #endregion 


    //first in last out collection
    private Stack<ShellBase> magazine = new Stack<ShellBase>();
    private ShellBase chamber;
    public static bool canFire = true;

    #region Sound variables
    //Sound variable
    public EventReference firingSound;
    public EventReference dryFireSound;
    public EventReference reloadSound;
    public EventReference pumpBackwardSound;
    public EventReference pumpForwardSound;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";

        SingleShotCrosshair.gameObject.SetActive(true);
        MultiShotCrosshair.gameObject.SetActive(false);

        //dw about it
        animator.SetBool("canFire", true);
    }

    // Update is called once per frame
    void Update()
    {
        // Changed Input.GetButton to Input.GetButtonDown, basically, no slam firing. I also did this because it spams the Dry Fire sound -V
        //that's so smart dude - N
        if (canFire && Input.GetButtonDown("Fire1")) Fire();

        //racking
        if (Input.GetButtonDown("Fire2"))
        {
            PlaySound(pumpBackwardSound);
            animator.CrossFade("Pump_Backwards", 0.2f);
            Debug.Log("R mouse down");
            canFire = false;

            if (chamber is not null)
            {
                ChamberUIOff();
            }

            chamber = null;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            PlaySound(pumpForwardSound);
            animator.CrossFade("Pump_Fwd", 0.2f);
            Debug.Log("R mouse up");
            if (magazine.Count > 0)
            { 
                chamber = magazine.Pop();
                float size = chamber.Size;
                MagLoss(chamber.Size);
                MagazineUILoss();
                //temporary based on current UI
                if (chamber.Type == ShellBase.ShellType.Buckshot) ChamberUIOn(Color.red);
                if (chamber.Type == ShellBase.ShellType.Slug) ChamberUIOn(Color.green);
            }
            canFire = true;
        }

        /**
        if (Input.GetKey(KeyCode.Tab))
        {
            //animator.Play("Reload_Start");
            animator.SetBool("shellWheelSelected", false);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        { 
            animator.SetBool("shellWheelSelected", true);
            //animator.Play("Reload_Finish");
        }
         */



        SwitchCrosshairUI();

        //Changed Inputs from "c, x" to number pads / alpha pads to select shells - Alex
        if (Input.GetKeyDown(KeyCode.Keypad1) | Input.GetKeyDown(KeyCode.Alpha1)) AddBuckshot();
        if (Input.GetKeyDown(KeyCode.Keypad2) | Input.GetKeyDown(KeyCode.Alpha2)) AddSlug();
    }

    private void SwitchCrosshairUI()
    {
        if (chamber is not null)
        {
            ShellBase top = chamber;
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

        //defaults to small crosshair for visibility
        if (chamber is null && magazine.Count <= 0)
        {
            SingleShotCrosshair.gameObject.SetActive(true);
            MultiShotCrosshair.gameObject.SetActive(false);
        }
    }


    private void MagazineUILoss()
    {
        // Transform out of bound error fix (5 + 1 in the chamber) -V
        if (currentCapacity < totalCapacity)
            //magazineUI.transform.GetChild((int)currentCapacity).gameObject.SetActive(false);
        Destroy(magazineUI.transform.GetChild((int)currentCapacity).gameObject);
        spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";
    }
    private void ChamberUIOn(Color shellColor)
    { 
        chamberShell.gameObject.SetActive(true);
        chamberShell.color = shellColor;
    }
    private void ChamberUIOff()
    { 
        chamberShell.gameObject.SetActive(false);
    }

    // Dedicating a function that just calls this so the code isn't full of these really long function calls -V
    /// <summary>
    /// Plays a sound from the game object that this script is attached to, in this case, the player
    /// </summary>
    /// <param name="eventReference"> The path to the FMOD sound event </param>
    private void PlaySound(EventReference eventReference)
    {
        RuntimeManager.PlayOneShotAttached(eventReference, this.gameObject);
    }

    private void PlaySound(string path)
    {
        RuntimeManager.PlayOneShotAttached(path, this.gameObject);
    }

    // will these work in the animation event once we have modular shells set up? -V
    private void HideShellModel()
    {
        Debug.Log("Shell Invisible!");
    }
    private void ShowShellModel()
    {
        Debug.Log("Shell Visible!");
    }


    public void LoadChamber(ShellBase shell)
    {
        if (currentCapacity + shell.Size <= totalCapacity)
        {
            magazine.Push(shell);
            float size = shell.Size;
            currentCapacity += size;

            spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";
            PlaySound(reloadSound);
        }

    }

    public void AddSlug()
    {
        Slug slug = new Slug();
        if (currentCapacity + slug.Size <= totalCapacity)
        {
            LoadChamber(slug);
            Debug.Log("slug pressed");

            MakeShellUI(slug);
            //move to LoadChamber() dependant on how we want to display what's in the chamber, if at all 
            //GameObject display = magazineUI.transform.GetChild((int)currentCapacity - 1).gameObject;
            //display.SetActive(true);
            //display.GetComponent<Image>().color = Color.green;

        }
    }

    public void AddBuckshot()
    {
        Buckshot buck = new Buckshot();
        if (currentCapacity + buck.Size <= totalCapacity)
        {
            LoadChamber(buck);
            Debug.Log("buck pressed");

            MakeShellUI(buck);
            //GameObject display = magazineUI.transform.GetChild((int)currentCapacity - 1).gameObject;
            //display.SetActive(true);
            //display.GetComponent<Image>().color = Color.red;
        }
    }

    public void AddHalfShell()
    { 
        HalfShell half = new HalfShell();
        if (currentCapacity + half.Size <= totalCapacity)
        {
            LoadChamber(half);
            Debug.Log("half shell pressed");
            MakeShellUI(half);
        }
    }

    private void MakeShellUI(ShellBase shell)
    {
        GameObject UIshell = new GameObject();
        Image display = UIshell.AddComponent<Image>();
        RectTransform UIShellRectTransform = UIshell.GetComponent<RectTransform>();
        UIShellRectTransform.SetParent(magazineUI.transform, false);
        //does nothing??
        UIShellRectTransform.anchoredPosition = magazineUI.GetComponent<RectTransform>().anchorMax;

        //put this somewhere better dumbass -N
        float xOffset = -38;
        float buffer = -10f;
        float shellWidth = 30f;
        float shellHeight = 70f;
        float halfShellHeight = 35f;
        float max = 150; //there is no way i can possibly explain what max is in words only -N

        Color color = shell.DisplayColor;
        UIshell.GetComponent<Image>().color = color;

        //set size based on full or half shell
        switch (shell.Type)
        {
            case ShellBase.ShellType.HalfShell:
                UIShellRectTransform.sizeDelta = new Vector2(shellWidth, halfShellHeight);
                break;
            default:
                UIShellRectTransform.sizeDelta = new Vector2(shellWidth, shellHeight);
                break;
        }

                //set position based on capacity, shell size, & buffer
                float y = 0;
                float temp = max;
                if (currentCapacity % 1 == 0)
                {
                    //even number/no singluar half shells
                    y = max - ((totalCapacity - currentCapacity) * 75);
                }
                else
                {
                    y = max - ((currentCapacity + 0.5f) * 75) + 40;
                }

        UIShellRectTransform.localPosition = new Vector3(0, y, 0);
        UIshell.SetActive(true);
    }


    public void Fire()
    {
        //check if chamber IS NULL *NOT* == null, trying to reference chamber directly will always equate to null
        if (canFire == false || chamber is null)
        {
            Debug.Log($"cannot fire: {canFire} or chamber is null");
            PlaySound(dryFireSound);
            return;
        }
        else 
        {
            animator.SetInteger("Shoot_Variation", Random.Range(0, 3));
            animator.SetTrigger("Fire");

            ShellBase shell = chamber;
            //MagazineUILoss();
            chamber = null;
            ChamberUIOff();
            //determine behavior of shot based on shell type
            PlaySound(firingSound);
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

                            if (hit.collider.gameObject.tag == "Enemy")
                            { 
                                Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
                                HitEnemy(hit, shell);
                            }
                            else if (hit.collider.gameObject.tag == "Breakable")
                            {
                                Instantiate(dust, hit.point, Quaternion.LookRotation(hit.normal));
                                HitBreakable(hit, shell);
                            }
                            else
                            {
                                Instantiate(dust, hit.point, Quaternion.LookRotation(hit.normal));
                            }
                        }

                    }

                    break;
                case ShellBase.ShellType.Slug:
                    if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunRange))
                    {
                        if (hit.collider.gameObject.tag == "Enemy")
                        {
                            Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
                            HitEnemy(hit, shell);
                        }
                        else if (hit.collider.gameObject.tag == "Breakable")
                        {
                            Instantiate(dust, hit.point, Quaternion.LookRotation(hit.normal));
                            HitBreakable(hit, shell);
                        }
                        else
                        {
                            Instantiate(dust, hit.point, Quaternion.LookRotation(hit.normal));
                        }
                    }
                    break;
            }

        }

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

    private void HitBreakable(RaycastHit hit, ShellBase shell)
    {
        Debug.Log(hit.transform.name);

        BreakableObject obj = hit.transform.GetComponent<BreakableObject>();

        if (obj != null)
        {
            obj.Damage(shell.Damage);
            Debug.Log("breakable hit");
        }
    }

    private void MagLoss(float shellSize) => currentCapacity -= shellSize;
}