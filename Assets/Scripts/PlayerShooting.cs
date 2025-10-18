using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using Unity.Cinemachine;

public class PlayerShooting : MonoBehaviour
{
    //AKA Gun class
    //https://www.youtube.com/watch?v=THnivyG0Mvo 

    public GameObject fpsCam;
    public CinemachineImpulseSource impulse;

    [SerializeField] private ParticleSystem blood;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ParticleSystem muzzleflash;
    [SerializeField] private Transform      shotgunMuzzleflashPos;

    public Animator animator;

    //Add [SerializeField] in front of anything that needs tweaking/balancing

    private float totalCapacity = 5;
    private float currentCapacity = 0; 
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
    [Tooltip("Fully Loaded Sound - Plays when you can't load more shells")]
    public EventReference fullyLoadedSound;
    //public EventReference pumpBackwardSound;
    //public EventReference pumpForwardSound;
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
            //PlaySound(pumpBackwardSound);
            animator.CrossFade("Pump_Backwards", 0.2f);
            canFire = false;

            if (chamber is not null)
            {
                ChamberUIOff();
            }

            chamber = null;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            //PlaySound(pumpForwardSound);
            animator.CrossFade("Pump_Fwd", 0.2f);
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

        if (Input.GetKeyDown(KeyCode.Tab)) {
            canFire = false;
            animator.SetBool("shellWheelSelected", true);
            animator.CrossFade("Reload_Start", 0.2f);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            canFire = true;
            animator.speed = 1;
            animator.SetBool("shellWheelSelected", false);
            animator.CrossFade("Reload_Finish", 0.2f);
        }



        if (Input.GetKey(KeyCode.Tab))
        {
            animator.speed = 1 / Time.timeScale;
            //animator.CrossFade("Reload_Start", 0.2f);
        }
        
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

    // Making the if checks regarding currentCapacity <= total capacity to this return method to save time later on -V
    // Following the KISS principal
    private bool CanLoad(float shellSize)
    {
        if (currentCapacity + shellSize <= totalCapacity)
        {
            return true;
        }
        PlaySound(fullyLoadedSound);
        return false;
    }

    public void LoadChamber(ShellBase shell)
    {
        if (CanLoad(shell.Size))
        {
            magazine.Push(shell);
            float size = shell.Size;
            currentCapacity += size;

            spaceLeftText.text = $"Can load {totalCapacity - currentCapacity} shells";
            //PlaySound(reloadSound);
            if (Input.GetKey(KeyCode.Tab)) // Do not play the reload animation if the player is loading via number keys (Alt reload animations for number keys?) -V
            {
                animator.CrossFade("reload_loop", 0.01f);
                animator.SetTrigger("LoadShell");
            }
            else
            {
                PlaySound(reloadSound);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Pump_Backwards") || animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Pumped")) animator.CrossFade("Empty_InsertShell", 0.2f);

        }

    }

    public void AddSlug()
    {
        Slug slug = new Slug();
        if (CanLoad(slug.Size))
        {
            LoadChamber(slug);
            magUI.Add(slug);
            LoadMagUI(slug);

        }
    }

    public void AddBuckshot()
    {
        Buckshot buck = new Buckshot();
        if (CanLoad(buck.Size))
        {
            LoadChamber(buck);
            magUI.Add(buck);
            LoadMagUI(buck);
        }
    }

    public void AddHalfShell()
    {
        HalfShell half = new HalfShell();
        if (CanLoad(half.Size))
        {
            LoadChamber(half);
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
            impulse.GenerateImpulse();
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
        //Debug.Log(hit.transform.name);

        IEnemy enemy = hit.transform.GetComponent<IEnemy>();

        if (enemy != null)
        {
            enemy.Damage(shell.Damage);
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

        GameObject UIshell = PlayerUI.MakeUIShell(ChamberUINOTSHELLUI, shell);
        UIshell.SetActive(true);
    }
    private void ChamberUIOff()
    {
        Destroy(ChamberUINOTSHELLUI.transform.GetChild(1).gameObject);
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
}