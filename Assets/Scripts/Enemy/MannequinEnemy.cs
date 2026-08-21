using System;
using UnityEngine;

/// <summary>
/// Mannequin Idle animations made into an enum list. To only be used with the mannequin enemy.
/// </summary>
public enum MannequinIdle
{
    Idle1, 
    Idle2, 
    Idle3, 
    Idle4, 
    Idle5, 
    Idle6, 
    Idle7, 
    Idle8
}

/// <summary>
/// Mannequin Cultist(?), a tortured soul given new meaning in life by fitting in with the people.
/// The "people" in question being mannequins. Will not attack the player on sight unless given the chance.
/// </summary>
public class MannequinEnemy : Enemy, IHasMeleeAttack
{

    [Header("Mannequin Enemy Properties")]
    // This enemy is still/static/fixed by default even when they spot the player, when they *actually* start chasing the player, this value will set the movement speed.
    public float combatMovementSpeed = 2f;

    [Tooltip("How close (preferrably within detection range, outside attack range) the player needs to be for this enemy to start moving to the player")]
    public float defaultEngageRange = 1f;
    [Tooltip("Whether or not the enemy will start chasing the player when they're within engagement range")]
    public bool canEngage = true;

    [Header("Dynamic")]
    [SerializeField] private float engageRange;
    [field:SerializeField] public bool PlayerInTrigger { get; set; } = false;

    [field:SerializeField] public bool isEngaging { get; private set; } = false;
    // Something to discuss maybe: Double range when the player isn't looking at the mannequin, making them "active" at a further distance when the player is facing away

    [SerializeField] private bool bPlayerSaw = false;
    [SerializeField] private float PlayerSawTime = 0f;
    [field: SerializeField] public LookScript lookComponent { get; private set; }

    public MannequinIdle idleAnim;
    private string[] idleAnims = new string[8];

    private float difficultyFOVScale { get { return ( Math.Clamp(160f - Player.playerCinemachineCamera.Lens.FieldOfView, 90, 160) + (10f * (4 - PlayerPrefs.GetInt("DIFFICULTY")))); } }
    private float distToPlayer
    {
        get { return (this.gameObject.transform.position - Player.gameObject.transform.position).magnitude; }
    }

    private float lastTwitchTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        engageRange = defaultEngageRange;
        idleAnims = Enum.GetNames(typeof(MannequinIdle));
        if (animator != null)
        { 
            animator.Play(idleAnims[(int)idleAnim]);
            lookComponent.UpdateHead();
        }
    }

    void Awake()
    {
        if (lookComponent == null)
            lookComponent = gameObject.GetComponent<LookScript>();
        base.Startup();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = ColorsExt.orange;
        Gizmos.DrawWireSphere(transform.position, defaultEngageRange);
    }

    // Update is called once per frame
    void Update()
    {
        if (stateMachine.CurrentState is ChaseState && !isEngaging)
        {
            if (!isEngaging)
            {
                Vector3 Dir = (Player.playerCinemachineCamera.gameObject.transform.position - lookComponent.headBone.position).normalized;
                float Angle = Vector3.Angle(Dir, Player.playerCinemachineCamera.gameObject.transform.forward);
                //Debug.Log("(Angle: " + Angle + ")" + "(Required Angle to Pass: " + difficultyFOVScale + ")");
                if (lookComponent.lookEnabled && (Mathf.Abs(Angle)) > difficultyFOVScale)
                {
                    if (!bPlayerSaw)
                    {
                        bPlayerSaw = true;
                        PlayerSawTime = UnityEngine.Random.Range(0.05f, 0.25f);
                    }
                    if (PlayerSawTime <= 0)
                    {
                        //Debug.Log("I am no longer looking");
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
                    if (Mathf.Abs(Angle) < difficultyFOVScale) // These two if statements should switch if you're debugging the angle
                    {
                        bPlayerSaw = false;
                        SpottedPlayer();
                        lookComponent.EnableLooking();
                    }
                }
                if (distToPlayer < engageRange)
                    EngagePlayer();
            }
        }

        base.BaseUpdate();
    }

    void FixedUpdate()
    {
        if (isEngaging)
        {
            if (Time.time > lastTwitchTime)
            {
                lastTwitchTime = Time.time + UnityEngine.Random.Range(1f, 8f);
                if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
                {
                    animator.SetTrigger("Twitch");
                }
            }
        }
    }

    public override bool SpottedPlayer()
    {
        lookComponent.ResetLookSpeed();
        lookComponent.SetFocusPoint(Player.playerCinemachineCamera.gameObject.transform);
        return false;
    }

    public void EngagePlayer()
    {
        isEngaging = true;
        animator.SetTrigger("StartMoving");
        movementSpeed = combatMovementSpeed;
        agent.speed = movementSpeed;
        agent.angularSpeed = 7200f;
        agent.acceleration = 80f;
    }

    public override void TakeDamage(float amount)
    {
        if (!isEngaging)
        {
            stateMachine.TransitionTo(stateMachine._chaseState);
            EngagePlayer();
        }
        base.TakeDamage(amount);
    }

    public void SetPlayerInTrigger(bool boolean)
    {
        PlayerInTrigger = boolean;
    }
}
