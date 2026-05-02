using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class SniperEnemy : RangedEnemy
{
    [SerializeField] ParticleSystem laserGlint;
    [SerializeField] ParticleSystem warningGlint;
    [SerializeField] ParticleSystem laserDot;
    [SerializeField] LineRenderer laser;
    [SerializeField] LineRenderer bulletTracer;

    // Shooting Behavior
    float time = 0;
    float step { get => Time.deltaTime; }
    [SerializeField] private float prefireTime = 1f;
    bool bShooting = false;
    bool bFired = false;
    float catchupTime = 0f;
    public Vector3 aimPos { get; private set; }
    LayerMask hitMask = 1 << 0 | 1 << 4 | 1 << 6 | 1 << 9 | 1 << 12;
    Transform gunChild;
    public bool bAimingAtPlayer { 
        get 
        {
            return Vector3.Distance(aimPos, Player.transform.position) < 0.05f; 
        }
    }

    [SerializeField] private EventReference sniperWarning;

    void Awake()
    {
        base.Startup();
        nextTimeToFire = setFireRate;
    }

    void Start()
    {
        aimPos = transform.forward * detectionRange;
        DisableLaser();
        gunChild = RecursiveFindChild(transform, "Pistol");
    }

    void Update()
    {
        base.BaseUpdate();

        if (Time.timeScale <= 0) return;

        UpdateAim();

        // Shooting State
        if (stateMachine.CurrentState == stateMachine._shootState)
        {
            if (bShooting)
            {
                if (time < prefireTime + 1.5f)
                {
                    time += step;

                    if (time > prefireTime && !bFired)
                    {
                        bFired = true;
                        nextTimeToFire = Time.time + setFireRate;
                        ActuallyShoot();
                    }
                    return;
                }
                else
                { 
                    FinishShoot();
                }
            }
        }
    }

    public override bool SpottedPlayer()
    {
        catchupTime = 0;
        nextTimeToFire = Time.time + setFireRate;
        EnableLaser();
        return true;
    }

    void UpdateAim()
    {
        if (bShooting || stateMachine.CurrentState != stateMachine._shootState) return;

        if (!bAimingAtPlayer)
        {
            // Let the sniper try aiming at the player first before firing
            if (Vector3.Distance(aimPos, Player.transform.position) < 5)
            {
                if (Time.time > nextTimeToFire)
                    nextTimeToFire = Time.time + step;
            }

            if (catchupTime < 1)
            {
                catchupTime += (step * 0.125f);
            }

            aimPos = Vector3.Lerp(aimPos, Player.transform.position, catchupTime);

            laser.SetPosition(0, gunChild.position);

            if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out RaycastHit hit, 9999, hitMask))
            {
                if (!laserDot.isEmitting)
                    laserDot.Play();
                laserDot.transform.position = hit.point;
                laser.SetPosition(1, hit.point);
            }
            else
            {
                laser.SetPosition(1, gameObject.transform.forward * 9999);
                if (laserDot.isEmitting)
                    laserDot.Stop();
            }
        }
    }

    public override void Shoot()
    {
        time = 0f;
        bShooting = true;
        DisableLaser();
        warningGlint.Play(true);
        if (!sniperWarning.IsNull)
            RuntimeManager.PlayOneShot(sniperWarning, transform.position);
    }

    void ActuallyShoot()
    {
        animator.Play("Pistol Shooting");
        
        muzzleflash.Play(true);

        if (!firingSound.IsNull)
            RuntimeManager.PlayOneShot(firingSound, transform.position);

        Vector3 inaccuracy = new Vector3(Random.Range(-ShotOffset,ShotOffset), Random.Range(-ShotOffset, ShotOffset), Random.Range(-ShotOffset, ShotOffset));

        LineRenderer bt = Instantiate<LineRenderer>(bulletTracer, gunChild.position, Quaternion.identity);

        bt.SetPosition(0, gunChild.position);

        if (Physics.Linecast(gunChild.position, aimPos + inaccuracy, out RaycastHit hit, hitMask))
        {
            bt.SetPosition(1, hit.point);

            if (hit.transform.gameObject.TryGetComponent<PlayerBehavior>(out PlayerBehavior _player))
                _player.TakeDamage(damage * Enemy.DamageMultiplier);
        }
        else
            bt.SetPosition(1, gameObject.transform.forward * 9999);
    }

    void FinishShoot()
    {
        bShooting = false; bFired = false;
        EnableLaser();
        catchupTime = 0;
    }

    public override void TakeDamage(float amount)
    {
        if (Health - amount <= 0 && !Dead)
        {
            KillSniperVFX();
        }

        base.TakeDamage(amount);
    }

    public void DisableLaser()
    {
        laser.gameObject.SetActive(false);
        laserDot.Stop();
        laserGlint.Stop(true);
    }
    public void EnableLaser()
    {
        laser.gameObject.SetActive(true);
        laserDot.Play();
        laserGlint.Play(true);
    }

    void KillSniperVFX()
    {
        warningGlint.gameObject.SetActive(false);
        laserDot.gameObject.SetActive(false);
        laserGlint.gameObject.SetActive(false);
        laser.gameObject.SetActive(false);
    }
}