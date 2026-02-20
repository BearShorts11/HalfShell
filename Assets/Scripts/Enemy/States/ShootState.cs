using FMODUnity;
using UnityEngine;

public class ShootState : State
{
    /// <summary>
    /// Cast owner to RangedEnemy as it gives access to RangedEnemy specific variables
    /// </summary>
    private RangedEnemy OwnerAsRanged;

    public ShootState(Enemy owner)
    {
        this.Owner = owner;
        OwnerAsRanged = owner as RangedEnemy;
    }

    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        Debug.Log("shooting state");

        //state changes
        if (Vector3.Distance(Owner.Player.transform.position, Owner.transform.position) > Owner.attackRange)
        {
            Owner.stateMachine.TransitionTo(Owner.stateMachine._chaseState);
            return;
        }

        //behavior
        switch (Owner)
        {
            case RangedEnemy:
                //logic for moving
                if (Vector3.Distance(Owner.Player.transform.position, Owner.transform.position) <= OwnerAsRanged.tooCloseRange)
                {
                    //Solution from https://discussions.unity.com/t/random-point-within-circle-with-min-max-radius/724904/10
                    Vector3 randomDirection = (Random.insideUnitCircle * Owner.Player.transform.position).normalized;
                    float randomDistance = Random.Range(OwnerAsRanged.tooCloseRange, OwnerAsRanged.maxDistanceFromPlayer);
                    Vector3 point = Owner.Player.transform.position + randomDirection * randomDistance;

                    Owner.agent.SetDestination(point);
                }
                break;
            default:
                //no movement- for sniper
                break;
        }

        //shooting timer
        if (Time.time >= OwnerAsRanged.nextTimeToFire)
        {
            OwnerAsRanged.nextTimeToFire = Time.time + 1f / OwnerAsRanged.fireRate;
            Shoot();
        }
        Owner.transform.LookAt(Owner.Player.transform);
    }

    private void Shoot()
    {
        Owner.animator.Play("Pistol Shooting");

        Transform gunChild = OwnerAsRanged.RecursiveFindChild(Owner.transform, "Pistol");

        //get object from the pool (eventually)
        GameObject bullet = GameObject.Instantiate(OwnerAsRanged.bulletPrefab);
        //set transform to that of enemy's gun (seperated for pooling)
        bullet.transform.position = gunChild.position;
        bullet.transform.rotation = gunChild.rotation;
        bullet.transform.parent = Owner.transform;

        Vector3 playerCurrPos = Owner.Player.transform.position;
        bullet.GetComponent<EnemyBullet>().GiveTarget(playerCurrPos);  
        //OwnerAsRanged.muzzleflash.Play(true); //VINH muzzleFlash isn't assigned and I don't know where to find it

        RuntimeManager.PlayOneShot(OwnerAsRanged.firingSound, Owner.transform.position);

    }
}
