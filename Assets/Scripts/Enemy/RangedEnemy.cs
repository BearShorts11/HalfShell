using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.UI.GridLayoutGroup;

public class RangedEnemy : Enemy, IHasRangedAttack
{
    [Header("Ranged Enemy Specific Variables")]
    public GameObject bulletPrefab;
    //private ObjectPool<GameObject> bulletPool; TODO
    /// <summary>
    /// If enemy is maked as free range (does  not use fire points), this is the distance at which the player is considered too close.
    /// AKA minimum distance between enemy and player
    /// </summary>
    public float tooCloseRange = 10f;
    public float setFireRate = 2f;
    public float nextTimeToFire = 0;

    public float minFireRate = 0.12f;
    public float maxFireRate = 1.2f;

    private int clipSize = 9;
    private int currentClip;
    private bool bReloading = false;

    /// <summary>
    /// offsets the shot randomly between +/- shot offset on all axises
    /// </summary>
    [SerializeField] private float _shotOffset;
    public float ShotOffset { get { return _shotOffset; } }

    public bool AllowSlugDrops = true;

    [Header("Fire Points Configuration")]
    public bool UseFirePoints;
    public List<Transform> FirePoints;
    private Transform currentPoint;

    [Header("Free Roam Configuration")]
    public float maxDistanceFromPlayer = 25f;

    [Header("VFX/Sounds")]
    // Muzzleflash must be assigned in the Unity Editor as an existing game object in the enemy prefab -V
    public ParticleSystem muzzleflash;
    public EventReference firingSound;

    private void Awake()
    {
        base.Startup();
        currentClip = clipSize;
    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();

        Voice_Update();
    }

    public override void TakeDamage(float amount)
    {
        if (Health - amount <= 0)
        {
            PlayVoice("event:/Enemy/EnemyDeath");
            base.TakeDamage(amount);
            return;
        }

        if (!statusEffected)
        {
            vocalCoolDown = 0.01f;
            if (!IsOnVocalCooldown())
            {
                PlayVoice("event:/Dialogue/cultistsDmg");
            }
        }

        if (agent.velocity.x > 0 || agent.velocity.z > 0)
        {
            animator.Play("Pistol Hit Running");
        }
        else
        {
            animator.Play("Pistol Hit Reaction");
        }

        base.TakeDamage(amount);
    }

    private void Reload()
    {
        animator.Play("Reload");
        nextTimeToFire += (animator.GetCurrentAnimatorStateInfo(1).length) + Random.Range(minFireRate, maxFireRate);
        Invoke(nameof(FinishReloading), animator.GetCurrentAnimatorStateInfo(1).length + 2f);
        bReloading = true;
    }

    private void FinishReloading()
    {
        currentClip = clipSize;
        bReloading = false;
    }

    public override void Shoot()
    {
        if (currentClip <= 0) 
        {
            if (bReloading) return;
            Reload();
            return; 
        }

        currentClip--;

        animator.Play("Pistol Shooting");

        //Transform gunChild =RecursiveFindChild(transform, "Pistol");

        //get object from the pool (eventually)
        GameObject bullet = GameObject.Instantiate(bulletPrefab);
        //set transform to that of enemy's gun (seperated for pooling)
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.transform.parent = transform;

        // Distance modifier made as an attempt to make a fixed cone of inaccuracy as an attempt to make ranged enemies a better shot up close with high ShotOffset values
        float accuracyMod = ShotOffset * (Vector3.Distance(this.gameObject.transform.position, Player.transform.position) / 15f);
        Vector3 accuracyOffset = new Vector3(
            UnityEngine.Random.Range(-accuracyMod, accuracyMod),
            UnityEngine.Random.Range(-accuracyMod, accuracyMod), 
            UnityEngine.Random.Range(-accuracyMod, accuracyMod)
            );
        Vector3 playerCurrPos = Player.transform.position + accuracyOffset;
        bullet.GetComponent<EnemyBullet>().GiveTarget(playerCurrPos);
        muzzleflash.Play(true);

        RuntimeManager.PlayOneShot(firingSound, transform.position);
    }

    public override bool SpottedPlayer()
    {
        if (base.SpottedPlayer()) return true;

        vocalCoolDown = 0.00001f;
        if (!IsOnVocalCooldown())
        {
            PlayVoice("event:/Dialogue/cultistBark");
        }
        return false;
    }

    void Voice_Update()
    {
        if (!IsOnVocalCooldown())
        {

            vocalCoolDown = defaultVocalCoolDown + Random.Range(-1.5f, 1.5f);

            if (statusEffected && statusEffectShell.Type == ShellBase.ShellType.Incindiary)
            {
                vocalCoolDown = 5f;
                PlayVoice("event:/Enemy/EnemyBurning");
            }

            /*switch (stateMachine.CurrentState)
            {
                case MeleeAttackState:
                    vocalCoolDown = 0.9f;
                    PlayVoice("event:/Dialogue/cultistsAtk");
                    break;
                    //case ChaseState:
                    //    PlayVoice("event:/Dialogue/cultistBark");
                    //    break;
            }*/

            //lastVocalization = Time.time;
        }
    }
}
