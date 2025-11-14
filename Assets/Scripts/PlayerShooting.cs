using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class PlayerShooting : MonoBehaviour
{
    //AKA Gun class
    //https://www.youtube.com/watch?v=THnivyG0Mvo 

    public GameObject fpsCam;
    public CinemachineImpulseSource impulse;
    public GameObject ApollyonBarks;

    public GameObject BulletHole;
    [SerializeField] private ParticleSystem blood;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ParticleSystem muzzleflash;
    [SerializeField] private Transform      shotgunMuzzleflashPos;

    public Animator animator;

    //Add [SerializeField] in front of anything that needs tweaking/balancing

    [SerializeField] private float pumpBackTime = 0.06f;
    [SerializeField] private float pumpFWDTime = 0.06f;
    [SerializeField] private float reloadRate = 0.15f;
    [SerializeField] private float chamberInsertTime = 0.5f;
    [SerializeField] private float gunReloadFinishTime = 0.25f;
    [SerializeField] private float inputBuffer = 0.01f;
    private float _WaitTime;
    private float waitTime
    {
        // Return the time left from a previous action
        get { return Mathf.Clamp(_WaitTime - Time.time, 0, Mathf.Infinity); }
        set { _WaitTime = value; }
    }

    private string lastMethod;
    private bool isInShellSelect // Bool that can only return a value
    {
        get { return ShellWheelController.shellWheelSelected; }
    }
    public bool lookingAtGun = false;
    private bool pumped = false;

    private float totalCapacity = 5;
    private float currentCapacity = 0; 
    [SerializeField] private float spreadRange = 0.1f; //variation in raycasts for non single shots (random spread)
    private float gunRange = 100f;

    public Dictionary<ShellBase.ShellType, int> AmmoCounts = new Dictionary<ShellBase.ShellType, int>() 
    {
        { ShellBase.ShellType.Buckshot, 0 },
        { ShellBase.ShellType.HalfShell, 0 },
        { ShellBase.ShellType.Slug, 0 }
    };

    [Header("Starting Ammo Counts")]
    public int startingHalfShells;
    public int startingSlugs;
    public int startingBuckshot;

    #region UI fields - move to own object

    [Header("UI")]
    TextMeshProUGUI spaceLeftText;
    public Image magazineUI;
    public Image SingleShotCrosshair;
    public Image MultiShotCrosshair;
    public GameObject ShellSelectionMenu;
    #endregion

    //realated but UI
    private PlayerUI playerUI;
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
    [Tooltip("Fully Loaded Sound - Plays when you can't load more shells")]
    public EventReference fullyLoadedSound;
    //public EventReference pumpBackwardSound;
    //public EventReference pumpForwardSound;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AmmoCounts[ShellBase.ShellType.Buckshot] = startingBuckshot; // Should be 0 but wtv
        AmmoCounts[ShellBase.ShellType.HalfShell] = startingHalfShells;
        AmmoCounts[ShellBase.ShellType.Slug] = startingSlugs;

        playerUI = FindFirstObjectByType<PlayerUI>();
        spaceLeftText = playerUI.currentCapacityText;
        spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";

        SingleShotCrosshair.gameObject.SetActive(true);
        MultiShotCrosshair.gameObject.SetActive(false);

        //dw about it
        animator.SetBool("canFire", true);

        List<GameObject> buttons = new List<GameObject>();
        buttons.AddRange(GameObject.FindGameObjectsWithTag("ShellButton"));
        foreach (GameObject button in buttons)
        {
            ShellSelectionButton counter = button.GetComponent<ShellSelectionButton>();
            counter.UpdateAmmoCount();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Looking at the face of the gun: cannot shoot or reload while looking at it.
        if (Input.GetKeyDown(KeyCode.F) && !isInShellSelect && !pumped) 
        {
            lookingAtGun = !lookingAtGun;
            ShellWheelController.shellWheelDisabled = !ShellWheelController.shellWheelDisabled;
            LookAtGun(lookingAtGun);
        }
        if (lookingAtGun) return;

        if (canFire && Input.GetButtonDown("Fire1")) Fire();
        
        

        //racking
        if (Input.GetButtonDown("Fire2"))
        {
            PumpBack();
        }
        if (Input.GetButtonUp("Fire2"))
        {
            PumpFWD();
        }

        animator.speed = Time.timeScale < 1 ? 1 / Time.timeScale : 1;
        // Can someone set a button name for these? - V  
        if (Input.GetKeyDown(KeyCode.Tab) | Input.GetKeyDown(KeyCode.LeftControl) ) 
        {
            GunRaise();
            if (lookingAtGun) lookingAtGun = false;
        }
        if (Input.GetKeyUp(KeyCode.Tab) | Input.GetKeyUp(KeyCode.LeftControl))
        {
            GunLower();
        }


        float magCount = magazine.Count;
        playerUI.SwitchCrosshairUI(chamber, magCount);

        //Changed Inputs from "c, x" to number pads / alpha pads to select shells - Alex
        if (Input.GetKeyDown(KeyCode.Keypad1) | Input.GetKeyDown(KeyCode.Alpha1)) AddHalfShell(); 
        
        if (Input.GetKeyDown(KeyCode.Keypad2) | Input.GetKeyDown(KeyCode.Alpha2)) AddSlug(); 
        
    }

    private void LookAtGun(bool looking)
    {
        if (looking) animator.CrossFade("Idle_Goto_LookAtFace", 0.1f);
        else animator.CrossFade("LookAtFace_Goto_Idle", 0.1f);
    }

    internal void GunRaise()
    {
        if (isWaiting())
        { 
            BufferLastFunction(nameof(GunRaise), waitTime);
            return;
        }
        canFire = false;
        lookingAtGun = false;
        animator.SetBool("shellWheelSelected", true);
        animator.CrossFade("Reload_Start", 0.2f);
    }

    private void GunLower()
    {
        if (isWaiting())
        {
            BufferLastFunction(nameof(GunLower), waitTime);
            return;
        }
        canFire = true;
        animator.speed = 1;
        animator.SetBool("shellWheelSelected", false);
        animator.CrossFade("Reload_Finish", 0.2f);
        SetWaitTime(gunReloadFinishTime);
    }

    private void PumpBack()
    {
        if (isWaiting())
        {
            BufferLastFunction(nameof(PumpBack), waitTime);
            return;
        }
        if (isInShellSelect)
            return;

        //PlaySound(pumpBackwardSound);
        animator.CrossFade("Pump_Backwards", 0.15f);
        canFire = false;

        if (chamber is not null)
        {
            playerUI.ChamberUIOff();
            ShellBase shell = chamber as ShellBase;
            AmmoCounts[shell.Type]++;
        }

        chamber = null;
        SetWaitTime(pumpBackTime);
        pumped = true;
    }
    private void PumpFWD()
    {
        if (isWaiting())
        {
            BufferLastFunction(nameof(PumpFWD), waitTime);
            return;
        }
        if (isInShellSelect)
            return;
        //PlaySound(pumpForwardSound);
        animator.CrossFade("Pump_Fwd", 0.15f);
        if (magazine.Count > 0)
        {
            chamber = magazine.Pop();
            float size = chamber.Size;
            MagLoss(chamber.Size);
            magUI.RemoveAt(magUI.Count - 1);
            MagazineUILoss();
            //temporary based on current UI
            playerUI.ChamberUIOn(chamber);
        }
        if (ShellWheelController.shellWheelSelected != true) { canFire = true; }
        SetWaitTime(pumpFWDTime);
        pumped = false;
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

    // Making the if checks regarding currentCapacity <= total capacity to this return method to save time later on -V
    // Following the KISS principal
    private bool CanLoad(ShellBase shell)
    {
        //technicially true, gonna need checks in loading instead for if chamber is empty
        if (shell.Type == ShellBase.ShellType.BMG) return false;

        float size = shell.Size;

        //can always use half shells
        if (shell.Type == ShellBase.ShellType.HalfShell && currentCapacity + size <= totalCapacity) return true;

        //check dictionary
        if (AmmoCounts[shell.Type] <= 0 || isWaiting())
        {
            PlaySound(fullyLoadedSound); //some kind of feedback for number key users
            return false;
        }

        // CHAMBER LOAD: the player still has the gun pumped back after inserting from the chamber instead of the usual feed, the player needs to pump the gun forward before making subsequent reloads.
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Empty_InsertShell") || animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_QuickReload_Pumped")) && pumped)
            return false;

        if (currentCapacity + size <= totalCapacity)
        {
            return true;
        }
        PlaySound(fullyLoadedSound);
        return false;
    }

    public void LoadMagazine(ShellBase shell)
    {
        if (CanLoad(shell))
        {
            magazine.Push(shell);
            float size = shell.Size;
            currentCapacity += size;

            spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";
            //PlaySound(reloadSound);
            if (Input.GetKey(KeyCode.Tab) | Input.GetKey(KeyCode.LeftControl)) 
            {
                //animator.CrossFade("reload_loop", 0.01f);
                animator.SetTrigger("LoadShell");
                SetWaitTime(reloadRate / animator.speed);
            }
            else // Play Alt reload animations for number keys
            {
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Pump_Backwards") || animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Pumped")) 
                {
                    animator.CrossFade("Empty_InsertShell", 0.2f);
                    SetWaitTime(chamberInsertTime);
                }
                else
                {
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Empty_InsertShell") || animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_QuickReload_Pumped"))
                    {
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_QuickReload_Pumped"))
                            animator.CrossFade("Idle_QuickReload_Pumped", 0.1f);
                        else
                            animator.CrossFade("Idle_QuickReload_Pumped", 0.05f, 0, 0.08f);
                    }
                    else
                    {
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_QuickReload"))
                            animator.CrossFade("Idle_QuickReload", 0.1f);
                        else
                            animator.CrossFade("Idle_QuickReload", 0.05f, 0, 0.08f);
                    }
                    SetWaitTime(reloadRate);
                }
                //PlaySound(reloadSound);
            }

        }

    }

    public void AddSlug()
    {
        Slug slug = new Slug();
        if (CanLoad(slug))
        {
            LoadMagazine(slug);
            magUI.Add(slug);
            LoadMagUI(slug);

            AmmoCounts[ShellBase.ShellType.Slug]--;
        }
    }

    public void AddBuckshot()
    {
        Buckshot buck = new Buckshot();
        if (CanLoad(buck))
        {
            LoadMagazine(buck);
            magUI.Add(buck);
            LoadMagUI(buck);

            AmmoCounts[ShellBase.ShellType.Buckshot]--;
        }
    }

    public void AddHalfShell()
    {
        HalfShell half = new HalfShell();
        if (CanLoad(half))
        {
            LoadMagazine(half);
            magUI.Add(half);
            LoadMagUI(half);

            //AmmoCounts[ShellBase.ShellType.HalfShell]--;
        }
    }


    public bool AddAmmo(int ammoCount, ShellBase shell)
    {
        if (AmmoCounts[shell.Type] < shell.MaxHolding)
        { 
            AmmoCounts[shell.Type] += ammoCount;
            if (AmmoCounts[shell.Type] > shell.MaxHolding) AmmoCounts[shell.Type] = shell.MaxHolding;
            return true;
        }
        return false;
    }


    public void Fire()
    {
        if (isWaiting())
        {
            BufferLastFunction(nameof(Fire), waitTime);
            return;
        }
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
            playerUI.ChamberUIOff();
            //determine behavior of shot based on shell type
            PlaySound(firingSound);
            float impulseRange = Random.Range(0.5f, 2f);
            impulse.GenerateImpulseWithForce(impulseRange);
            //Debug.Log("ImpulseForce:" + impulseRange);
            playerUI.UIRattle(2);
            Destroy(ApollyonBarks);
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
                        if (Physics.Raycast(fpsCam.transform.position, fwd, out hit, gunRange) && hit.distance <= shell.MaxRange)
                        {
                            DoHit(hit, shell);
                        }
                    }

                    break;
                case ShellBase.ShellType.Slug:
                    if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunRange) && hit.distance <= shell.MaxRange)
                    {
                        DoHit(hit, shell);
                    }
                    break;
            }

        }

    }

    private void DoHit(RaycastHit hit, ShellBase shell)
    {
        Debug.Log("hit successful");
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
            SpawnBulletHole(hit);
        }
        else if (hit.collider.gameObject.GetComponent<ObjActivator>())
        {
            HitObjActivator(hit, shell.Type);
            SpawnBulletHole(hit);
        }
        else
        {
            Instantiate(dust, hit.point, Quaternion.LookRotation(hit.normal));
            SpawnBulletHole(hit);
        }
    }

    //https://www.youtube.com/shorts/mkIRV4nLOWo 
    private void SpawnBulletHole(RaycastHit hit)
    {
        GameObject decal = Instantiate(BulletHole, hit.point + (hit.normal * 0.1f), Quaternion.FromToRotation(Vector3.up, hit.normal));
        Destroy(decal, 3f);
    }

    private void HitEnemy(RaycastHit hit, ShellBase shell)
    {
        //Debug.Log(hit.transform.name);

        Limb eLimb = hit.transform.GetComponent<Limb>();
        if (eLimb != null) { eLimb.TakeDamage(shell.ScaleDamage(hit)); return; } // If it detects a limb was hit here, stop at this point since the limb script already calls the damage method to the enemy script

        IEnemy enemy = hit.transform.GetComponent<IEnemy>();

        if (enemy != null)
        {
            enemy.Damage(shell.ScaleDamage(hit));
            //Debug.Log("enemy hit");
        }
    }

    private void HitBreakable(RaycastHit hit, ShellBase shell)
    {
        //Debug.Log(hit.transform.name);

        BreakableObject obj = hit.transform.GetComponent<BreakableObject>();

        if (obj != null)
        {
            obj.Damage(shell.Damage);
            //Debug.Log("breakable hit");
        }
    }

    private void HitObjActivator(RaycastHit hit, ShellBase.ShellType shellType)
    {
        ObjActivator objActivator = hit.transform.GetComponent<ObjActivator>();
        if (objActivator != null)
        {
            objActivator.ObjSwap(shellType);
        }
    }

    private void MagLoss(float shellSize) => currentCapacity -= shellSize;


    private void MagazineUILoss()
    {
        // Transform out of bound error fix (5 + 1 in the chamber) -V
        if (currentCapacity < totalCapacity)
            Destroy(magazineUI.transform.GetChild(magazine.Count).gameObject);
        spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";
    }

    private void LoadMagUI(ShellBase shell)
    {
        GameObject UIshell = PlayerUI.MakeUIShell(magazineUI, shell);

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

        //Debug.Log(y);
        UIshell.GetComponent<RectTransform>().localPosition = new Vector3(0, y, 0);
        UIshell.SetActive(true);
    }

    public bool ShellInChamber() => chamber is not null;

    private void BufferLastFunction(string methodName, float time)
    {
        if (lastMethod == methodName)
            return;
        CancelInvoke();
        time += inputBuffer;
        Invoke(methodName, time);
        Invoke(nameof(ClearLastMethod), time + inputBuffer);
    }

    private void ClearLastMethod()
    {
        lastMethod = "";
    }

    private void SetWaitTime(float time)
    {
        waitTime = Time.time + time;
    }

    private bool isWaiting()
    {
        return Time.time < _WaitTime;
    }
}