using UnityEngine;

public class MannequinEnemy : Enemy, IHasMeleeAttack
{
    [field:SerializeField] public bool PlayerInTrigger { get; set; } = false;

    [SerializeField] private bool isChasing { get; set; } = false;
    [SerializeField] private bool bPlayerSaw = false;
    [SerializeField] private float PlayerSawTime = 0f;
    [field: SerializeField] public LookScript lookComponent { get; private set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void Awake()
    {
        if (lookComponent == null)
            lookComponent = gameObject.GetComponent<LookScript>();
        base.Startup();
    }

    // Update is called once per frame
    void Update()
    {
        if (stateMachine.CurrentState is ChaseState)
        {
            Vector3 Dir = (Player.playerCinemachineCamera.gameObject.transform.position - transform.position).normalized;
            float Angle = Vector3.SignedAngle(Dir, Player.playerCinemachineCamera.gameObject.transform.forward, Player.playerCinemachineCamera.gameObject.transform.up);

            if (lookComponent.lookEnabled && (Mathf.Abs(Angle)) > Player.playerCinemachineCamera.Lens.FieldOfView + 15f)
            {
                if (!bPlayerSaw)
                {
                    bPlayerSaw = true;
                    PlayerSawTime = Random.Range(0.05f, 0.25f);
                }
                if (PlayerSawTime <= 0)
                {
                    Debug.Log("I am no longer looking");
                    lookComponent.SetLookSpeed(lookComponent.defaultLookSpeed * 2f);
                    lookComponent.DisableLooking();
                }
                else if (bPlayerSaw && PlayerSawTime > 0)
                {
                    PlayerSawTime -= Time.deltaTime;
                }
            }
            else if (!lookComponent.lookEnabled)
            {
                //Debug.Log("I should be looking. (Angle: "+Angle+")");
                if (Mathf.Abs(Angle) < Player.playerCinemachineCamera.Lens.FieldOfView + 15f) // These two if statements should switch if you're debugging the angle
                {
                    bPlayerSaw = false;
                    SpottedPlayer();
                    lookComponent.EnableLooking();
                }
            }
        }

        base.BaseUpdate();
    }

    public override bool SpottedPlayer()
    {
        lookComponent.ResetLookSpeed();
        lookComponent.SetFocusPoint(Player.playerCinemachineCamera.gameObject.transform);
        return false;
    }

    public void SetPlayerInTrigger(bool boolean)
    {
        PlayerInTrigger = boolean;
    }
}
