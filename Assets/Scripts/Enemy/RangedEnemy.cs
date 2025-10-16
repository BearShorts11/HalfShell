using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemy : IEnemy
{
    public List<Transform> shootingPoints;
    [SerializeField] private float shootingDistance= 10f;
    public GameObject bulletPrefab;
    /// <summary>
    /// fire rate is shots per second
    /// </summary>
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float turnSpeed = 5f;
    private float nextTimeToFire = 0;

    public static float gunDamage = 15f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Startup();
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
        }

        //look towards/track player
        transform.LookAt(player.transform);
    }

    private void Shoot()
    { 
        Instantiate(bulletPrefab, this.transform.position, this.transform.rotation);
    }

    [ContextMenu("Damage")]
    public void DamageTest()
    {
        base.Damage(10f);
    }
}
