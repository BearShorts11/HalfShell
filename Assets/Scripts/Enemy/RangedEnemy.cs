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

    public float minFireRate = 0.5f;
    public float maxFireRate = 1.5f;

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
    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();
    }

    public override void TakeDamage(float amount)
    {
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

    public override void Shoot()
    {
        animator.Play("Pistol Shooting");

        Transform gunChild =RecursiveFindChild(transform, "Pistol");

        //get object from the pool (eventually)
        GameObject bullet = GameObject.Instantiate(bulletPrefab);
        //set transform to that of enemy's gun (seperated for pooling)
        bullet.transform.position = gunChild.position;
        bullet.transform.rotation = gunChild.rotation;
        bullet.transform.parent = transform;

        Vector3 playerCurrPos = Player.transform.position + new Vector3(UnityEngine.Random.Range(-ShotOffset, ShotOffset),
            UnityEngine.Random.Range(-ShotOffset, ShotOffset), UnityEngine.Random.Range(-ShotOffset, ShotOffset));
        bullet.GetComponent<EnemyBullet>().GiveTarget(playerCurrPos);
        muzzleflash.Play(true);

        RuntimeManager.PlayOneShot(firingSound, transform.position);
    }
}
