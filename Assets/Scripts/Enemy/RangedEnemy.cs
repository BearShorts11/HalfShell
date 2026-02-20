using FMODUnity;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class RangedEnemy : Enemy
{

    [Header("Ranged Enemy Specific Variables")]
    public GameObject bulletPrefab;
    //private ObjectPool<GameObject> bulletPool; TODO
    /// <summary>
    /// If enemy is maked as free range (does  not use fire points), this is the distance at which the player is considered too close.
    /// AKA minimum distance between enemy and player
    /// </summary>
    public float tooCloseRange = 10f;
    public float fireRate = 0.5f;
    public float nextTimeToFire = 0;

    [Header("Fire Points Configuration")]
    public bool UseFirePoints;
    public List<Transform> FirePoints;
    private Transform currentPoint;

    [Header("Free Roam Configuration")]
    public float maxDistanceFromPlayer = 25f;

    [Header("VFX/Sounds")]
    //public ParticleSystem muzzleflash;
    public EventReference firingSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
}
