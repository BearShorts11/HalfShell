using UnityEngine;

public class FindFirePointState : State
{
    /// <summary>
    /// Cast owner to RangedEnemy as it gives access to RangedEnemy specific variables
    /// </summary>
    private RangedEnemy OwnerAsRanged;
    private Transform currentPoint;

    public FindFirePointState(Enemy owner) 
    { 
        this.Owner = owner;
        OwnerAsRanged = Owner as RangedEnemy;
    }

    public override void Enter()
    {
        FindNewCover();
    }

    public override void Exit()
    {
    }

    public override void Update()
    {
    }

    private void FindNewCover()
    {
        Transform newPoint = OwnerAsRanged.FirePoints[Random.Range(0, OwnerAsRanged.FirePoints.Count)];
        while (newPoint.position == currentPoint.position)
        {
            newPoint = OwnerAsRanged.FirePoints[Random.Range(0, OwnerAsRanged.FirePoints.Count)];
        }

        currentPoint = newPoint;
        Debug.Log($"New point: {currentPoint.gameObject.name}");
        //navigate to cover
        NavigateToCover();
    }

    private void NavigateToCover()
    {
        if (currentPoint is null) return;

        if (Vector3.Distance(Owner.transform.position, currentPoint.position) < 2f)
        {
            Owner.stateMachine.TransitionTo(Owner.stateMachine._shootState);
        }

        Owner.agent.SetDestination(currentPoint.position);
    }

    private void GetNearestCover()
    {
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
    }
}
