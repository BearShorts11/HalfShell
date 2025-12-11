using FMODUnity;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.AI;

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

    private RagdollController ragdollController;
    private Animator gunAnimator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gunAnimator = GetComponentInChildren<Animator>();
        ragdollController = GetComponentInChildren<RagdollController>();

        Startup();

        if (shootingPoints.Count > 1)
        {
            GetNearestCover();
            state = State.findCover;
        }
    }

    // Update is called once per frame
    void Update()
    {
        AnimationController();

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        //Debug.Log(state);

        switch (state)
        {
            case State.idle:
                // Detetcts if the player is within detetction radius.
                if (distanceToPlayer <= shootingDistance)
                {
                    state = State.shoot;
                }
                break;
            case State.shoot:
                if (Time.time >= nextTimeToFire)
                {
                    nextTimeToFire = Time.time + 1f / fireRate;
                    Shoot();
                }
                break;
            case State.findCover:
                NavigateToCover();
                break;
            case State.chasing:
                state = State.shoot;
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

        Transform gunChild = this.transform.GetChild(0);
        GameObject bullet = Instantiate(bulletPrefab, gunChild.position, gunChild.rotation);
        Vector3 playerCurrPos = player.transform.position;
        bullet.GetComponent<EnemyBullet>().GiveTarget(playerCurrPos);
        //Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);   

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        if (playerDistance <= tooClose && shootingPoints.Count > 1)
        {
            //Debug.Log("too close!");
            FindNewCover();
            state = State.findCover;
        }

        RuntimeManager.PlayOneShot("event:/Weapons/Enemies/Pistol/Pistol_Fire", this.gameObject.transform.position);
        
    }

    public override void Damage(float damageAmt)
    {
        gunAnimator.Play("Pistol Hit Reaction");
        if (state == State.dead)
        {
            ragdollController.ApplyForceToRagdoll(damageAmt);
        }
        base.Damage(damageAmt);
    }

    private void AnimationController()
    {
        //Controls Idle/Walking/Running 
        if (state == State.idle)
        {
           gunAnimator.SetFloat("Speed", 0.0f);
        }
        else if (state == State.patrol)
        {
            //animator.SetFloat("Speed", 0.5f);
        }
        else if (state == State.chasing)
        {
            //animator.SetFloat("Speed", 1f);
        }
        else
        {
            //animator.SetFloat("Speed", 0.0f);
        }

        if (agent.velocity.z > 0)
        {
            gunAnimator.Play("Pistol Running");
        }
        else if(agent.velocity.z < 0)
        {
            gunAnimator.Play("Pistol Running Backwards");
        }

        if (agent.velocity.x > 0 && state == State.findCover)
        {
            //Debug.Log("Navmesh Move Right");
            gunAnimator.Play("Pistol Strafe R");
        }
        else if (agent.velocity.x < 0 && state == State.findCover)
        {
            //Debug.Log("NavAgent Move Left");
            gunAnimator.Play("Pistol Strafe L");
        }

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
