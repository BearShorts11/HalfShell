using FMODUnity;
using UnityEngine;

public class ShootState : State
{
    /// <summary>
    /// Cast owner to RangedEnemy as it gives access to RangedEnemy specific variables
    /// </summary>
    private RangedEnemy OwnerAsRanged;

    /// <summary>
    /// If using fire points, holds the current point enemy is at or pathing to
    /// </summary>
    private Transform currentPoint;

    private bool foundNewPoint;

    public ShootState(Enemy owner)
    {
        this.Owner = owner;
        OwnerAsRanged = owner as RangedEnemy;
    }

    public override void Enter()
    {
        if (Owner is RangedEnemy && OwnerAsRanged.UseFirePoints) GetNearestFirePoint();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
        //state changes
        if (!OwnerAsRanged.UseFirePoints && Vector3.Distance(Owner.Player.transform.position, Owner.transform.position) > Owner.attackRange)
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
                    if (OwnerAsRanged.UseFirePoints)
                    {
                        //Debug.Log($"found new point {foundNewPoint}");
                        if (!foundNewPoint) FindNewFirePoint();
                    }
                    else
                    { 
                        //Solution from https://discussions.unity.com/t/random-point-within-circle-with-min-max-radius/724904/10
                        Vector3 randomDirection = (Random.insideUnitCircle * Owner.Player.transform.position).normalized;
                        float randomDistance = Random.Range(OwnerAsRanged.tooCloseRange, OwnerAsRanged.maxDistanceFromPlayer);
                        Vector3 point = Owner.Player.transform.position + randomDirection * randomDistance;

                        Owner.agent.SetDestination(point);
                    }
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

    /// <summary>
    /// Finds a random fire point and navigates to it
    /// </summary>
    private void FindNewFirePoint()
    {
        Transform newPoint = OwnerAsRanged.FirePoints[Random.Range(0, OwnerAsRanged.FirePoints.Count)];
        while (newPoint.position == currentPoint.position)
        {
            newPoint = OwnerAsRanged.FirePoints[Random.Range(0, OwnerAsRanged.FirePoints.Count)];
        }
        //Debug.Log($"found new point: {newPoint.name}");
        currentPoint = newPoint;
        foundNewPoint = true;
        //navigate to cover
        NavigateToCover();
    }

    private void NavigateToCover()
    {
        if (currentPoint is null) return;
        
        foundNewPoint = false;

        if (Vector3.Distance(Owner.transform.position, currentPoint.position) <= 3f)
        {
            //Debug.Log("at point");
            return;
        }

        //Debug.Log("Navigating to point");
        Owner.agent.SetDestination(currentPoint.position);
    }

    /// <summary>
    /// Gets the nearest fire point and navigates to it
    /// </summary>
    private void GetNearestFirePoint()
    {
        //Debug.Log("Finding nearest point");

        int index = 0;
        float currentClosest = Vector3.Distance(Owner.transform.position, OwnerAsRanged.FirePoints[0].position);
        for (int i = 1; i < OwnerAsRanged.FirePoints.Count; i++)
        {
            if (OwnerAsRanged.FirePoints[i] is null)
            {
                Debug.Log("ERROR: fill up your shooting points list (empty/null space)");
                continue;
            }

            float currDistance = Vector3.Distance(Owner.transform.position, OwnerAsRanged.FirePoints[i].position);
            if (currDistance < currentClosest)
            {
                index = i;
                currentClosest = currDistance;
            }
        }
        currentPoint = OwnerAsRanged.FirePoints[index];
        NavigateToCover();
    }
}
