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
    [SerializeField] private ParticleSystem muzzleflash;
    [SerializeField] private Transform      shotgunMuzzleflashPos;

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
    public Image ChamberUINOTSHELLUI;
    public Image SingleShotCrosshair;
    public Image MultiShotCrosshair;
    public GameObject ShellSelectionMenu;
    #endregion 

    //realated but UI
    float shellUIstart;
    List<ShellBase> magUI = new List<ShellBase>();

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
                magUI.RemoveAt(magUI.Count - 1);
                MagazineUILoss();
                //temporary based on current UI
                ChamberUIOn(chamber);
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
        //can I remove this? -N


        SwitchCrosshairUI();

        //Changed Inputs from "c, x" to number pads / alpha pads to select shells - Alex
        if (Input.GetKeyDown(KeyCode.Keypad1) | Input.GetKeyDown(KeyCode.Alpha1)) AddHalfShell();
        if (Input.GetKeyDown(KeyCode.Keypad2) | Input.GetKeyDown(KeyCode.Alpha2)) AddSlug();
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
            magUI.Add(slug);
            LoadMagUI(slug);

        }
    }

    public void AddBuckshot()
    {
        Buckshot buck = new Buckshot();
        if (currentCapacity + buck.Size <= totalCapacity)
        {
            LoadChamber(buck);
            Debug.Log("buck pressed");
            magUI.Add(buck);
            LoadMagUI(buck);
        }
    }

    public void AddHalfShell()
    { 
        HalfShell half = new HalfShell();
        if (currentCapacity + half.Size <= totalCapacity)
        {
            LoadChamber(half);
            Debug.Log("half shell pressed");
            magUI.Add(half);
            LoadMagUI(half);
        }
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
            if (muzzleflash != null)
                Instantiate(muzzleflash, shotgunMuzzleflashPos);
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
                case ShellBase.ShellType.HalfShell:

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
                case ShellBase.ShellType.HalfShell:
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
            Destroy(magazineUI.transform.GetChild(magazine.Count).gameObject);
        spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";
    }
    private void ChamberUIOn(ShellBase shell)
    {

        GameObject UIshell = MakeUIShell(ChamberUINOTSHELLUI, shell);
        UIshell.SetActive(true);
    }
    private void ChamberUIOff()
    {
        Destroy(ChamberUINOTSHELLUI.transform.GetChild(1).gameObject);
    }

    private void LoadMagUI(ShellBase shell)
    {
        GameObject UIshell = MakeUIShell(magazineUI, shell);

        //set position based on capacity, shell size, & buffer
        float y = 0;
        float size = shell.Size;
        y = 160;
        if (shell.Type == ShellBase.ShellType.HalfShell) y += 17.5f; //half of halfshell height
        shellUIstart = y;

        for (int i = 1; i < magUI.Count; i++)
        {
            if (magUI[i - 1].Type == ShellBase.ShellType.HalfShell) y -= 40;
            else y -= 75;
        }

        Debug.Log(y);
        UIshell.GetComponent<RectTransform>().localPosition = new Vector3(0, y, 0);
        UIshell.SetActive(true);
    }

    public GameObject MakeUIShell(Image parent, ShellBase shell)
    {
        GameObject UIshell = new GameObject();
        Image display = UIshell.AddComponent<Image>();
        RectTransform UIShellRectTransform = UIshell.GetComponent<RectTransform>();
        UIShellRectTransform.SetParent(parent.transform, false);

        //put this somewhere better dumbass -N
        float shellWidth = 30f;
        float shellHeight = 70f;
        float halfShellHeight = 35f;

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

        return UIshell;
    }
}