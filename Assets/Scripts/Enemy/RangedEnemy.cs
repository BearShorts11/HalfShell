using FMODUnity;
using NUnit.Framework;
using PixelCrushers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.Pool;
using static PixelCrushers.DialogueSystem.UnityGUI.GUIProgressBar;

public class RangedEnemy : IEnemy
{
    [SerializeField] private float shootingDistance = 25;
    [SerializeField] private float tooClose = 10f;
    public GameObject bulletPrefab;
    /// <summary>
    /// fire rate is shots per second
    /// </summary>
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float turnSpeed = 5f;
    private float nextTimeToFire = 0;

    public static float gunDamage = 5f;

    public List<Transform> shootingPoints;
    private Transform currentPoint;

    public bool followsPlayer;
    public float minDistanceFromPlayer = 10f;
    public float maxDistanceFromPlayer = 25f;

    private RagdollController ragdollController;
    private Animator gunAnimator;
    [SerializeField] ParticleSystem muzzleflash;


    private ObjectPool<GameObject> bulletPool;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunAnimator = GetComponentInChildren<Animator>();
        ragdollController = GetComponentInChildren<RagdollController>();

        Startup();

        if (followsPlayer) state = State.idle;
        else if (shootingPoints.Count > 1)
        {
            state = State.findCover;
            GetNearestCover();
        }
        else state = State.idle;

        // Create a pool with the four core callbacks.
        bulletPool = new ObjectPool<GameObject>(
            createFunc: CreateItem,
            actionOnGet: OnGet,
            actionOnRelease: OnRelease,
            actionOnDestroy: OnDestroyItem,
            collectionCheck: true,   // helps catch double-release mistakes
            defaultCapacity: 5, //really enemy should never need more than like 3
            maxSize: 8
        );
    }

    #region pool behaviors

    private GameObject CreateItem()
    { 
        return Instantiate( bulletPrefab );
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
        Destroy( gameObject );
    }

    #endregion

    // Update is called once per frame
    void Update()
    {
        AnimationController();

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(state);
        if (Docile)
        {
            state = State.idle;
            return;
        }

        switch (state)
        {
            case State.idle:
                // Detetcts if the player is within detetction radius.
                if (distanceToPlayer <= shootingDistance)
                {
                    if (followsPlayer) state = State.chasing;
                    else state = State.shoot;
                }
                break;
            case State.shoot:
                if (followsPlayer) state = State.chasing;
                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / fireRate;
                    Shoot();
                }
                break;
            case State.findCover:
                NavigateToCover();
                if (followsPlayer) state = State.chasing;
                break;
            case State.chasing:
                if (followsPlayer == false) state = State.shoot;
                Chase();
                break;
        }

        //look towards/track player
        //find a way to restrict to only y rotation
        if (state != State.dead)
        {
            transform.LookAt(player.transform);
        }
    }

    protected override void Chase()
    {
        //logic for moving

        if (Vector3.Distance(player.transform.position, this.transform.position) <= tooClose)
        { 
            //https://discussions.unity.com/t/random-point-within-circle-with-min-max-radius/724904/10
            Vector3 randomDirection = (Random.insideUnitCircle * player.transform.position).normalized;
            float randomDistance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            Vector3 point = player.transform.position + randomDirection * randomDistance;
        
            agent.SetDestination(point);
        }


        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    private void FindNewCover()
    {
        //choose a new cover: random
        //Debug.Log("Finding new cover");
        Transform newPoint = shootingPoints[Random.Range(0, shootingPoints.Count)];
        while (newPoint.position == currentPoint.position)
        {
            newPoint = shootingPoints[Random.Range(0, shootingPoints.Count)];
        }

        currentPoint = newPoint;
        Debug.Log($"New point: {currentPoint.gameObject.name}");
        //navigate to cover
        NavigateToCover();
    }

    private void NavigateToCover()
    {
        //new cover found
        //Debug.Log("Navigating");
        if (currentPoint is null) return;

        if (Vector3.Distance(this.transform.position, currentPoint.position) < 2f)
        {
            state = State.idle;
            return;
        }

        agent.SetDestination(currentPoint.position);
    }

    private void GetNearestCover()
    {
        int index = 0;
        float currentClosest = Vector3.Distance(this.transform.position, shootingPoints[0].position);
        for (int i = 1; i < shootingPoints.Count; i++)
        {
            if (shootingPoints[i] is null) Debug.Log("ERROR: fill up your shooting points list (empty/null space)");
            //throw new ArgumentException("Wasp is in patrol mode and path list is empty. Please add some waypoints or take it out of patrol mode.");

            float currDistance = Vector3.Distance(this.transform.position, shootingPoints[i].position);
            if (currDistance < currentClosest)
            {
                index = i;
                currentClosest = currDistance;
            }
        }
        currentPoint = shootingPoints[index];
    }

    private void Shoot()
    {
        gunAnimator.Play("Pistol Shooting");

        Transform gunChild = RecursiveFindChild(this.transform, "Pistol");
        Debug.Log(gunChild == null);

        //GameObject bullet = Instantiate(bulletPrefab, gunChild.position, gunChild.rotation);
        //get object from the pool
        GameObject bullet = bulletPool.Get();
        //set transform to that of enemy's gun
        bullet.transform.position = gunChild.position;
        bullet.transform.rotation = gunChild.rotation;
        //return to pool after 5s (aka deactivate)
        StartCoroutine("ReturnAfter", bullet);

        Vector3 playerCurrPos = player.transform.position;
        bullet.GetComponent<EnemyBullet>().GiveTarget(playerCurrPos);
        //Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);   
        muzzleflash.Play(true);

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        if (!followsPlayer && playerDistance <= tooClose && shootingPoints.Count > 1)
        {
            //Debug.Log("too close!");
            FindNewCover();
            state = State.findCover;
        }

        RuntimeManager.PlayOneShot("event:/Weapons/Enemies/Pistol/Pistol_Fire", this.gameObject.transform.position);
        
    }

    private IEnumerable ReturnAfter(GameObject bullet)
    { 
        yield return new WaitForSeconds(5f);
        bulletPool.Release(bullet);

    }

    //https://stackoverflow.com/questions/33437244/find-children-of-children-of-a-gameobject
    Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }


    public override void Damage(float damageAmt)
    {
        if (state == State.dead)
        {
            ragdollController.ApplyForceToRagdoll(damageAmt);
        }

        if (agent.velocity.x > 0 ||  agent.velocity.z > 0)
        {
            gunAnimator.Play("Pistol Hit Running");
        }
        else
        {
            gunAnimator.Play("Pistol Hit Reaction");
        }

        base.Damage(damageAmt);   
    }

    private void AnimationController()
    {
        gunAnimator.SetFloat("Velocity X", agent.velocity.x);
        gunAnimator.SetFloat("Velocity Y", agent.velocity.z);

        //Controls Death and Ragdoll

        if (state == State.dead)
        {
            //enemyLogic.enabled = false;
            gunAnimator.enabled = false;
            ragdollController.SetColliderState(true);
            ragdollController.SetRigidbodyState(false);
            return;
        }

    }
}
