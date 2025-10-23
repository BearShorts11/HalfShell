using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : IEnemy
{
    [SerializeField] private float shootingDistance = 50f;
    [SerializeField] private float tooClose = 10f;
    public GameObject bulletPrefab;
    /// <summary>
    /// fire rate is shots per second
    /// </summary>
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float turnSpeed = 5f;
    private float nextTimeToFire = 0;

    public static float gunDamage = 15f;

    public List<Transform> shootingPoints;
    private Transform currentPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Debug.Log(state);

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
                    nextTimeToFire = Time.time + 1f/fireRate;
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
        transform.LookAt(player.transform);
    }

    private void FindNewCover()
    {
        //choose a new cover: random
        currentPoint = shootingPoints[Random.Range(0, shootingPoints.Count)];
            Debug.Log($"new point found: {currentPoint.gameObject.name}");

        //navigate to cover
        NavigateToCover();
    }

    private void NavigateToCover()
    {
        //new cover found
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
        Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);
        if (playerDistance <= tooClose && shootingPoints.Count > 1)
        {
            Debug.Log("too close!");
            FindNewCover();
            state = State.findCover;
        }
    }
}
