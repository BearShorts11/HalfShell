using Assets.Scripts;
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PlayerShooting : MonoBehaviour
{
    //AKA Gun class
    //https://www.youtube.com/watch?v=THnivyG0Mvo 

    public GameObject fpsCam;
    public CinemachineImpulseSource impulse;
    public GameObject ApollyonBarks;
    public Vector3 hitPosition;
    private LayerMask triggerMask;
    private SlowMo_Manager slowmo;
    public Enemy lastDamaged { get; set; }

    #region VFX
    //Impacts
    public GameObject BulletHole;
    [SerializeField] private ParticleSystem dust;
    [SerializeField] private ParticleSystem wood;
    [SerializeField] private ParticleSystem concrete;
    [SerializeField] private ParticleSystem metal;
    [SerializeField] private ParticleSystem blood;
    [SerializeField] private ParticleSystem incendiaryFX;

    [SerializeField] private ParticleSystem muzzleflash;
    [SerializeField] private Transform      shotgunMuzzleflashPos;
    [SerializeField] private Transform ejectionPoint;
    [SerializeField] private Mesh ejectedHalfShellMesh;
    public GameObject shellGO;
    public GameObject halfShellGO;
    private MeshRenderer shellMesh;
    private Material shellMaterial;
    private GameObject reloadedShellGO;

    public Animator animator;

    private ObjectPool<GameObject> bulletHolePool;

    #endregion

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
    protected bool isInShellSelect // Bool that can only return a value
    {
        get { return ShellWheelController.shellWheelSelected; }
    }
    public bool lookingAtGun = false;
    private bool pumped = false;

    public float totalCapacity = 5;
    public float currentCapacity = 0; 
    [SerializeField] private float spreadRange = 0.1f; //variation in raycasts for non single shots (random spread)
    private float gunRange = 100f;
    private bool useShells = true; //toggle infinite shells

    public Dictionary<ShellBase.ShellType, int> AmmoCounts = new Dictionary<ShellBase.ShellType, int>() 
    {
        { ShellBase.ShellType.Incindiary, 0 },
        { ShellBase.ShellType.Buckshot, 0 },
        { ShellBase.ShellType.HalfShell, 0 },
        { ShellBase.ShellType.Slug, 0 }
    };

    [Header("Starting Ammo Counts")]
    public int startingHalfShells;
    public int startingSlugs;
    public int startingIncindiary;

    #region UI fields - move to own object

    [Header("UI")]

    /// <summary>
    /// Magazine background UI image
    /// </summary>
    public Image magazineUI;
    public Image SingleShotCrosshair;
    public Image MultiShotCrosshair;
    public GameObject ShellSelectionMenu;

    //realated but UI
    private PlayerUI playerUI;

    /// <summary>
    /// list of shells to be parsed through to create images for shell UI
    /// </summary>
    public List<ShellBase> magUI = new List<ShellBase>();
    #endregion

    //first in last out collection
    public Stack<ShellBase> Magazine { get; private set; } = new Stack<ShellBase>();
    public ShellBase Chamber { get; private set; }
    private ShellBase.ShellType ChamberType;
    public static bool canFire = true;

    #region new input handling
    public PlayerInput input;
    bool rack_performed;
    bool rack_cancled;
    #endregion

    #region Sound variables
    //Sound variable
    private EventInstance soundInstance;
    public EventReference firingSound;
    public EventReference dryFireSound;
    public EventReference reloadSound;
    [Tooltip("Fully Loaded Sound - Plays when you can't load more shells")]
    public EventReference fullyLoadedSound;
    //public EventReference pumpBackwardSound;
    //public EventReference pumpForwardSound;

    public EventReference bulletImpactEvent;
    private PARAMETER_ID impactSurfaceParamID;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AmmoCounts[ShellBase.ShellType.Buckshot] = startingIncindiary; // Should be 0 but wtv
        AmmoCounts[ShellBase.ShellType.HalfShell] = startingHalfShells;
        AmmoCounts[ShellBase.ShellType.Slug] = startingSlugs;

        playerUI = FindFirstObjectByType<PlayerUI>();

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

        
        input = GetComponent<PlayerInput>();

        bulletHolePool = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,   // helps catch double-release mistakes
            defaultCapacity: 100,
            maxSize: 500
        );

        RuntimeManager.StudioSystem.lookupPath(bulletImpactEvent.Guid, out string eventPath);

        EventDescription eventDesc;
        RuntimeManager.StudioSystem.getEvent(eventPath, out eventDesc);

        PARAMETER_DESCRIPTION paramDesc;
        eventDesc.getParameterDescriptionByName("ImpactSurface", out paramDesc);

        impactSurfaceParamID = paramDesc.id;
        triggerMask = ~LayerMask.GetMask("Trigger", "Ignore Raycast", "Player");

        slowmo = GetComponent<SlowMo_Manager>();
    }

    #region pool behaviors

    private GameObject CreateItem()
    {
        return Instantiate(BulletHole);
    }

    private void OnGet(GameObject bullet)
    {
        gameObject.SetActive(true);
    }

    private void OnRelease(GameObject bullet)
    {
        gameObject.SetActive(false);
    }

    private void OnDestroyItem(GameObject bullet)
    {
        Destroy(gameObject);
    }

    #endregion


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(AmmoCounts[ShellBase.ShellType.Incindiary]);

        if (PauseMenu.paused) { animator.speed = 0; return; }

        // Looking at the face of the gun: cannot shoot or reload while looking at it.
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isInShellSelect && !pumped) 
        {
            Debug.Log("LeftControl hit Shooting script");
            lookingAtGun = !lookingAtGun;
            
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
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            GunRaise();
            if (lookingAtGun) lookingAtGun = false;
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            GunLower();
        }


        float magCount = Magazine.Count;
        playerUI.SwitchCrosshairUI(Chamber, magCount);

        //Changed Inputs from "c, x" to number pads / alpha pads to select shells - Alex
        if (Input.GetKeyDown(KeyCode.Keypad1) | Input.GetKeyDown(KeyCode.Alpha1)) AddHalfShell(); 
        
        if (Input.GetKeyDown(KeyCode.Keypad2) | Input.GetKeyDown(KeyCode.Alpha2)) AddSlug(); 

        if (Input.GetKeyDown(KeyCode.Keypad3) | Input.GetKeyDown(KeyCode.Alpha3)) AddIncindiary(); 
        
    }

    public void LookAtGun(bool looking)
    {
        ShellWheelController.shellWheelDisabled = !ShellWheelController.shellWheelDisabled;
        if (looking) animator.CrossFade("Idle_Goto_LookAtFace", 0.1f);
        else animator.CrossFade("LookAtFace_Goto_Idle", 0.1f);
    }
    //Narrative Method Only
    public void ForceLookAtGun()
    {
        lookingAtGun = !lookingAtGun;
        LookAtGun(lookingAtGun);
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
        if (isInShellSelect)
            return;

        //PlaySound(pumpBackwardSound);
        animator.CrossFade("Pump_Backwards", 0.15f);
        canFire = false;

        if (Chamber is not null)
        {
            playerUI.ChamberUIOff();
            ShellBase shell = Chamber as ShellBase;
            if (useShells && AmmoCounts[shell.Type] < shell.MaxHolding)
            { 
                AmmoCounts[shell.Type]++;
                AlertController.SetRackAlert(shell.Type);
            }
        }

        Chamber = null;
        pumped = true;
    }
    private void PumpFWD()
    {
        if (isInShellSelect)
            return;
        //PlaySound(pumpForwardSound);
        animator.CrossFade("Pump_Fwd", 0.15f);
        if (Magazine.Count > 0)
        {
            Chamber = Magazine.Pop();
            float size = Chamber.Size;
            MagLoss(Chamber.Size);
            playerUI.MagazineUILoss(magUI[0]);
            magUI.RemoveAt(0);
            //temporary based on current UI
            playerUI.ChamberUIOn(Chamber);
            ChamberType = Chamber.Type;
        }
        if (ShellWheelController.shellWheelSelected != true) { canFire = true; }
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

    // Dedicated firing sound function since it's using parameters now... -V
    /// <summary>
    /// Play the Firing Sound with the parameter applied based on the shell type to play specific sounds or to modify the firing sound
    /// </summary>
    /// <param name="shelltype">Shell Type Enum, will be converted into an Interger as an ID</param>
    private void PlayFireSound(ShellBase.ShellType shelltype = 0)
    {
        soundInstance = RuntimeManager.CreateInstance(firingSound);
        soundInstance.set3DAttributes(RuntimeUtils.To3DAttributes(this.gameObject));
        RuntimeManager.AttachInstanceToGameObject(soundInstance, this.gameObject);
        if (shelltype != 0)
            soundInstance.setParameterByName("ShellType", (int)shelltype - 1);
        soundInstance.start();
        soundInstance.release();
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

        if (isWaiting() && !SetFromLoad)
        {
            PlaySound(fullyLoadedSound); //some kind of feedback for number key users
            return false;
        }

        //check dictionary
        if (AmmoCounts[shell.Type] <= 0)
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
        //prevents load errors
        if (Magazine is null) Magazine = new Stack<ShellBase>();

        if (CanLoad(shell))
        {
            Magazine.Push(shell);
            float size = shell.Size;
            currentCapacity += size;
            Color shellColor = Color.white;

            // Is this a half shell or a full shell
            if (shell.Type == ShellBase.ShellType.HalfShell)
            {
                halfShellGO.SetActive(true);
                shellGO.SetActive(false);
                reloadedShellGO = halfShellGO;
            }
            else
            {
                halfShellGO.SetActive(false);
                shellGO.SetActive(true);
                reloadedShellGO = shellGO;
            }
            // What color is the shell
            switch (shell)
            {
                case Buckshot:
                case HalfShell:
                    shellColor = Color.red;
                    break;
                case Slug:
                    shellColor = Color.green;
                    break;
                case Incindiary:
                    // WHAT DO YOU MEAN THERE'S NO ORANGE -V to Unity
                    shellColor = ColorsExt.orange;
                    break;
                case BMG:
                case BeanBag:
                default:
                    break;
            }

            shellMesh = reloadedShellGO.GetComponent<MeshRenderer>();
            shellMaterial = shellMesh.materials[1];
            shellMaterial.SetColor("_Color", shellColor);

            //PlaySound(reloadSound);
            if (Input.GetKey(KeyCode.Tab)) 
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
                            animator.CrossFade("Idle_QuickReload_Pumped", 0.02f);
                        else
                            animator.CrossFade("Idle_QuickReload_Pumped", 0.05f, 0, 0.08f);
                    }
                    else
                    {
                        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_QuickReload"))
                            animator.CrossFade("Idle_QuickReload", 0.02f);
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
            magUI.Insert(0, slug);
            //magUI.Add(slug);
            playerUI.LoadMagUI(slug);

            if (useShells) AmmoCounts[ShellBase.ShellType.Slug]--;
        }
    }

    public void AddBuckshot()
    {
        Buckshot buck = new Buckshot();
        if (CanLoad(buck))
        {
            LoadMagazine(buck);
            magUI.Insert(0, buck);
            //magUI.Add(buck);
            playerUI.LoadMagUI(buck);

            if (useShells) AmmoCounts[ShellBase.ShellType.Buckshot]--;
        }
    }

    public void AddHalfShell()
    {
        HalfShell half = new HalfShell();
        if (CanLoad(half))
        {
            LoadMagazine(half);
            magUI.Insert(0, half);
            //magUI.Add(half);
            playerUI.LoadMagUI(half);

            //AmmoCounts[ShellBase.ShellType.HalfShell]--;
        }
    }

    public void AddIncindiary()
    { 
        Incindiary fire = new Incindiary();
        if (CanLoad(fire))
        {
            LoadMagazine(fire);
            magUI.Insert(0, fire);
            //magUI.Add(fire);
            playerUI.LoadMagUI(fire);

            if (useShells) AmmoCounts[ShellBase.ShellType.Incindiary]--;
        }
    }


    public bool AddAmmo(int ammoCount, ShellBase shell)
    {
        //Debug.Log(shell.Type);
        if (AmmoCounts[shell.Type] < shell.MaxHolding)
        { 
            AmmoCounts[shell.Type] += ammoCount;
            if (AmmoCounts[shell.Type] > shell.MaxHolding) AmmoCounts[shell.Type] = shell.MaxHolding; //Mathf.clamp?
            //Debug.Log($"{shell.Type}, {AmmoCounts[shell.Type]}");
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
        if (canFire == false || Chamber is null)
        {
            //Debug.Log($"cannot fire: {canFire} or chamber is null");
            PlaySound(dryFireSound);
            return;
        }
        else 
        {
            if (muzzleflash != null)
                Instantiate(muzzleflash, shotgunMuzzleflashPos);
            else
                muzzleflash.Play(true);

            //animator.SetInteger("Shoot_Variation", Random.Range(0, 3));
            //animator.SetTrigger("Fire");
            animator.CrossFade("Shoot", 0.1f);

            ShellBase shell = Chamber;
            //MagazineUILoss();
            Chamber = null;
            playerUI.ChamberUIOff();
            //determine behavior of shot based on shell type
            
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
                case ShellBase.ShellType.Incindiary:

                    for (int i = 1; i <= shell.AmtProjectiles; i++)
                    {
                        //https://discussions.unity.com/t/raycast-bullet-spread/753464 
                        Vector3 fwd = fpsCam.transform.forward;
                        fwd += fpsCam.transform.TransformDirection(new Vector3(Random.Range(-spreadRange, spreadRange), Random.Range(-spreadRange, spreadRange)));
                        if (Physics.Raycast(fpsCam.transform.position, fwd, out hit, gunRange, triggerMask, QueryTriggerInteraction.Collide) && hit.distance <= shell.MaxRange)
                        {
                            if (shell.Type == ShellBase.ShellType.Incindiary)
                            {
                                float angle = Vector3.Angle(fwd,hit.normal);
                                if (angle > 175f)
                                {
                                    fwd -= fwd;
                                }
                                Instantiate(incendiaryFX, hit.point, Quaternion.LookRotation(hit.normal + fwd));
                            }
                            DoHit(hit, shell);
                        }
                    }

                    break;
                case ShellBase.ShellType.Slug:
                    if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, gunRange, triggerMask, QueryTriggerInteraction.Collide) && hit.distance <= shell.MaxRange)
                    {
                        DoHit(hit, shell);
                    }
                    break;
            }
            PlayFireSound(shell.Type);
        }

    }

    private void DoHit(RaycastHit hit, ShellBase shell)
    {
        PlayImpactSound(hit);
        Debug.DrawLine(fpsCam.transform.position, hit.point, Color.red, 5f);

        if (hit.collider.gameObject.tag == "Player") return; // I'm suspecting the player capsule is blocking the bullets when sprinting + backpedaling, especially when looking at the playtest footage

        if (hit.collider.gameObject.tag == "Enemy")
        {
            hitPosition = hit.point;
            Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
            HitEnemy(hit, shell);
        }
        else if (hit.collider.gameObject.tag == "Breakable")
        {
            SpawnImpactFX(hit);
            HitBreakable(hit, shell, shell.Type);
            SpawnBulletHole(hit);
        }
        else if (hit.collider.gameObject.GetComponent<ObjActivator>())
        {
            HitObjActivator(hit, shell.Type);
            SpawnBulletHole(hit);
        }
        else
        {
            SpawnImpactFX(hit);
            SpawnBulletHole(hit);
        }
    }

    private void SpawnImpactFX(RaycastHit hit)
    {
        int surfaceTypeID = 0;
        bool hasRender = hit.collider.gameObject.TryGetComponent<Renderer>(out Renderer renderer);

        if (!hasRender) renderer = hit.collider.gameObject.GetComponentInChildren<Renderer>(false); // Try looking for a render component in children if this is one of *those* game objects.

        if (renderer == null) // this mf thing still null
            renderer = hit.collider.gameObject.GetComponentInParent<Renderer>(false);

        if (renderer != null) // if this thing still null, I give up -_-
            surfaceTypeID = MaterialSurfaceTypeChecker.GetSurfaceType(renderer.sharedMaterial);

        switch (surfaceTypeID)
        {
            case 2:
                Instantiate(wood, hit.point, Quaternion.LookRotation(hit.normal));
                break;
            case 3:
                Instantiate(concrete, hit.point, Quaternion.LookRotation(hit.normal));
                break;
            case 4:
                Instantiate(metal, hit.point, Quaternion.LookRotation(hit.normal));
                break;
            case 5:
                Instantiate(blood, hit.point, Quaternion.LookRotation(hit.normal));
                break;
            case 0:
            case 1:
            default:
                Instantiate(dust, hit.point, Quaternion.LookRotation(hit.normal));
                break;
        }
    }

    //https://www.youtube.com/shorts/mkIRV4nLOWo 
    private void SpawnBulletHole(RaycastHit hit)
    {
        if (BulletHole == null) 
        {
            Debug.Log("Oi! BulletHole property is not set!");
            return; 
        }

        Quaternion normal = Quaternion.LookRotation(-hit.normal, Vector3.up);
        Quaternion rotation = Quaternion.Euler(0,0,Random.Range(0f,360f));

        //GameObject decal = Instantiate(BulletHole, hit.point + (hit.normal * 0.1f), normal * rotation, hit.transform);
        GameObject decal = bulletHolePool.Get();
        decal.transform.position = hit.point + (hit.normal * 0.1f);
        decal.transform.rotation = rotation * normal;
        decal.transform.SetParent(hit.transform);
    }

    private void HitEnemy(RaycastHit hit, ShellBase shell)
    {
        if (hit.collider.gameObject.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            float Damage = shell.ScaleDamage(hit);
            Enemy enemy = hit.collider.gameObject.transform.GetComponent<Enemy>();
            Limb limb = hit.collider.gameObject.transform.GetComponent<Limb>();
            
            // Get the enemy component from the Limb if a limb was hit
            if (enemy == null && limb != null)
            {
                enemy = limb.enemy;
            }

            if (enemy != null)
            {
                enemy.HitFrom(shell);
                if (shell.hasSpecialEffects)
                { 
                    enemy.HitEffect(shell);
                    Debug.Log("hit effect on enemy");
                }
            }

            if (enemy != null)
            {
                SetLastDamaged(enemy);
            }

            damageable.TakeDamage(Damage);
        }

        //Limb eLimb = hit.transform.GetComponent<Limb>();
        //if (eLimb != null) { eLimb.TakeDamage(shell.ScaleDamage(hit)); return; } // If it detects a limb was hit here, stop at this point since the limb script already calls the damage method to the enemy script

        //IEnemy enemy = hit.transform.GetComponent<IEnemy>();

        //if (enemy != null)
        //{
        //    enemy.Damage(shell.ScaleDamage(hit));
        //    //Debug.Log("enemy hit");
        //}
    }

    private void RewardKill()
    {
        if (slowmo != null)
        {
            slowmo.slowMoChance += 0.0025f;
        }
    }

    public void SetLastDamaged(Enemy enemy)
    {
        // Is this a new target or an old target
        if (lastDamaged != enemy)
        {
            if (slowmo == null) return;

            if (lastDamaged != null)
            { 
                lastDamaged.OnDeath.RemoveListener(slowmo.DramaEvent);
                CancelInvoke(nameof(Enemy_RemoveKillReward));
            }
            lastDamaged = enemy;
            lastDamaged.OnDeath.AddListener(slowmo.DramaEvent);
            lastDamaged.OnDeath.AddListener(RewardKill);
            Invoke(nameof(Enemy_RemoveKillReward), 1f);
        }
    }

    void Enemy_RemoveKillReward()
    {
        if (lastDamaged != null)
        {
            lastDamaged.OnDeath.RemoveListener(slowmo.DramaEvent);
            lastDamaged.OnDeath.RemoveListener(RewardKill);
            lastDamaged = null;
        }
    }

    private void HitBreakable(RaycastHit hit, ShellBase shell, ShellBase.ShellType shellType)
    {
        BreakableObject obj = hit.transform.GetComponent<BreakableObject>();
        if (obj.destructionOveride == false) { obj.DestructionPos = gameObject.transform.position; }

        if (obj != null) 
        {
            if (obj.shellSpecific && shellType != obj.targetShell) return;
            obj.TakeDamage(shell.Damage);
        }
    }

    private void HitObjActivator(RaycastHit hit, ShellBase.ShellType shellType)
    {
        ObjActivator objActivator = hit.transform.GetComponent<ObjActivator>();
        if (objActivator != null) { objActivator.ObjSwap(shellType); }
    }

    private void MagLoss(float shellSize) => currentCapacity -= shellSize;

    public bool ShellInChamber() => ChamberType is not 0;

    /// <summary>
    /// called from Kerth/PlayerData on scene reloaded. Recieves a stack of ints and converts them to shells
    /// </summary>
    public void SetMagazine(int[] reversedMagazine)
    {
        SetFromLoad = true;
        if (reversedMagazine is null) return;

        for (int i = reversedMagazine.Length - 1; i >= 0; i--)
        { 
            //casting for clarity
            ShellBase.ShellType type = (ShellBase.ShellType)reversedMagazine[i];

            switch (type)
            {
                case ShellBase.ShellType.HalfShell:
                    AddHalfShell();
                    break;
                case ShellBase.ShellType.Slug:
                    AmmoCounts[ShellBase.ShellType.Slug]++; //add slug/load mag needs 
                    AddSlug();
                    break;
            }
        }
        SetFromLoad = false;
    }
    bool SetFromLoad;

    /// <summary>
    /// called from Kerth/PlayerData on scene reloaded. Sets what is in the chamber
    /// </summary>
    public void SetChamber(ShellBase shell)
    { 
        if (shell is not null)
        { 
            Chamber = shell;
            if (playerUI is not null) playerUI.ChamberUIOn(Chamber);
        }
    }

    /// <summary>
    /// called from Kerth/PlayerData on scene reloaded. Sets AmmoCounts
    /// </summary>
    public void SetAmmoCounts(int[] ammoCounts)
    {
        if (ammoCounts is null || ammoCounts.Length == 0) return;

        //this fires before Start() meaning that they get reset...
        //still keeping both in just in case
        startingSlugs = ammoCounts[(int)ShellBase.ShellType.Slug];

        this.AmmoCounts[ShellBase.ShellType.HalfShell] = ammoCounts[(int)ShellBase.ShellType.HalfShell];
        this.AmmoCounts[ShellBase.ShellType.Slug] = ammoCounts[(int)ShellBase.ShellType.Slug];
    }

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

    public void InfiniteShells() => useShells = false;
    public void UseShells() => useShells = true;

    public void GunInspect()
    {
        animator.CrossFade("Draw_Inspect", 0f); //this plays Inspector Anim
    }
    private void PlayImpactSound(RaycastHit hit)
    {
        float surfaceValue = 0f;
        //if (hit.collider.CompareTag("DirtFloor"))
        //    surfaceValue = 0f;
        //else if (hit.collider.CompareTag("GravelFloor"))
        //    surfaceValue = 1f;
        //else if (hit.collider.CompareTag("WoodFloor"))
        //    surfaceValue = 2f;
        //else if (hit.collider.CompareTag("AsphaltFloor"))
        //    surfaceValue = 3f;
        //else if (hit.collider.CompareTag("MetalFloor"))
        //    surfaceValue = 4f;
        //else if (hit.collider.CompareTag("Enemy"))
        //    surfaceValue = 5f;
        bool hasRender = hit.collider.gameObject.TryGetComponent<Renderer>(out Renderer renderer);

        if (!hasRender) renderer = hit.collider.gameObject.GetComponentInChildren<Renderer>(); // Try looking for a render component in children if this is one of *those* game objects.

        if (renderer == null) // this mf thing still null
            renderer = hit.collider.gameObject.GetComponentInParent<Renderer>();

        if (renderer != null) // I hate this.
            surfaceValue = MaterialSurfaceTypeChecker.GetSurfaceType(renderer.sharedMaterial);
        
        if (hit.collider.CompareTag("Enemy")) // Because it can't get the enemy's mesh renderer properly -_-
            surfaceValue = 5f;

        // Debug.Log($"Surface Value: {surfaceValue}");
        EventInstance impact = RuntimeManager.CreateInstance(bulletImpactEvent);

        // play sound where the bullet hit
        impact.set3DAttributes(RuntimeUtils.To3DAttributes(hit.point));

        // set FMOD parameter for surface type
        impact.setParameterByID(impactSurfaceParamID, surfaceValue);

        impact.getParameterByID(impactSurfaceParamID, out float value);
        // Debug.Log("FMOD Parameter Set To: " + value);
        impact.start();
        impact.release();
    }

    public void EjectShell()
    {
        if (!ShellInChamber()) return;

        GameObject shell = Instantiate(Resources.Load<GameObject>("Shell_Ejection/Shell_Generic"), ejectionPoint.position, Quaternion.LookRotation(ejectionPoint.right));
        MeshFilter mesh = shell.transform.GetChild(0).GetComponent<MeshFilter>();
        Material material = shell.transform.GetChild(0).GetComponent<MeshRenderer>().materials[1];
        switch (ChamberType)
        {
            case ShellBase.ShellType.BMG:
            case ShellBase.ShellType.BeanBag:
            default:
                break;
            case ShellBase.ShellType.Buckshot:
                material.SetColor("_Color", Color.red);
                break;
            case ShellBase.ShellType.HalfShell:
                material.SetColor("_Color", Color.red);
                mesh.mesh = ejectedHalfShellMesh;
                break;
            case ShellBase.ShellType.Slug:
                material.SetColor("_Color", Color.green);
                break;
            case ShellBase.ShellType.Incindiary:
                material.SetColor("_Color", ColorsExt.orange);
                break;
        }
        ChamberType = 0;
        shell.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.right * Random.Range(150f, 300f));
        shell.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.up * Random.Range(75f, 210f));
        shell.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-15f, 15f), Random.Range(30f, 50f), Random.Range(-5f, -5f)));
    }
}