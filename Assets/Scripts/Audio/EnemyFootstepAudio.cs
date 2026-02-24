using UnityEngine;
using UnityEngine.AI;
using FMODUnity;
using FMOD.Studio;

public class NPCFootstepAudio : MonoBehaviour
{
    // ===== FMOD EVENTS =====
    [Header("FMOD Events")]
    [SerializeField] private EventReference footstepEvent;
    [SerializeField] private EventReference landEvent;
    [SerializeField] private string surfaceParameterName = "SurfaceTerrain";

    // ===== SETTINGS =====
    [Header("Settings")]
    [SerializeField] private float stepInterval = 0.5f; // Time between footsteps
    [SerializeField] private float minMoveSpeed = 0.05f; // Minimum speed to trigger footsteps
    [SerializeField] private float raycastDistance = 2f; // How far below NPC to check for ground
    [SerializeField] private LayerMask groundMask; // Which layers count as ground

    private NavMeshAgent agent;

    // Used to measure NPC movement between frames
    private Vector3 previousPosition;

    // Smoothed movement speed
    private float movementSpeed;

    private float stepTimer;
    private bool wasGrounded;
    private int currentSurfaceIndex;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("NPCFootstepAudio requires a NavMeshAgent.");
            enabled = false;
            return;
        }

        previousPosition = transform.position;
        stepTimer = stepInterval;

       
    }

    void Update()
    {
        bool isGrounded = CheckGrounded();

        // Calculate actual movement speed based on position delta
        movementSpeed = Vector3.Distance(transform.position, previousPosition) / Time.deltaTime;

        

        DetectSurface();

        // ===== LANDING =====
        if (isGrounded && !wasGrounded)
        {
           // Debug.Log("Landing detected!");
            PlayLandingSound();
        }

        // ===== FOOTSTEPS =====
        if (isGrounded)
        {
            if (movementSpeed >= minMoveSpeed)
            {
                stepTimer -= Time.deltaTime;
               

                if (stepTimer <= 0f)
                {
                    //Debug.Log("Playing FOOTSTEP sound.");
                    PlayFootstepSound();
                    stepTimer = stepInterval;
                }
            }
            else
            {
                
                stepTimer = stepInterval;
            }
        }
        else
        {
          
            stepTimer = stepInterval;
        }

        wasGrounded = isGrounded;
        previousPosition = transform.position;

       
       
    }

    // ===== GROUND CHECK =====
    private bool CheckGrounded()
    {
        // Get the bottom of the capsule
        Vector3 origin = transform.position + Vector3.up * 0.1f; // start slightly above ground
        float distance = raycastDistance;

        // Cast straight down
        bool grounded = Physics.Raycast(origin, Vector3.down, out RaycastHit hit, distance, groundMask);

       

        return grounded;
    }

    // ===== SURFACE DETECTION =====
    private void DetectSurface()
    {
        if (Physics.Raycast(
            transform.position + Vector3.up * 0.2f,
            Vector3.down,
            out RaycastHit hit,
            raycastDistance,
            groundMask))
        {
            string tag = hit.collider.tag;
           

            switch (tag)
            {
                case "DirtFloor":
                    currentSurfaceIndex = 0;
                    break;
                case "GravelFloor":
                    currentSurfaceIndex = 1;
                    break;
                case "WoodFloor":
                    currentSurfaceIndex = 2;
                    break;
                case "AsphaltFloor":
                    currentSurfaceIndex = 3;
                    break;
                case "MetalFloor":
                    currentSurfaceIndex = 4;
                    break;
                default:
                    currentSurfaceIndex = 0;
                    break;
            }

           
        }
        else
        {
            Debug.Log("No ground hit detected for surface detection!");
        }
    }

    // ===== FMOD =====
    private void PlayFootstepSound()
    {
        //Debug.Log("NPC FOOTSTEP PLAYED");

        EventInstance stepInstance = RuntimeManager.CreateInstance(footstepEvent);
        stepInstance.set3DAttributes(
            RuntimeUtils.To3DAttributes(transform.position)
        );
        stepInstance.setParameterByName(surfaceParameterName, currentSurfaceIndex);
        stepInstance.start();
        stepInstance.release();
    }

    private void PlayLandingSound()
    {
        //Debug.Log("NPC LANDING PLAYED");

        EventInstance landInstance = RuntimeManager.CreateInstance(landEvent);
        landInstance.set3DAttributes(
            RuntimeUtils.To3DAttributes(transform.position)
        );
        landInstance.setParameterByName(surfaceParameterName, currentSurfaceIndex);
        landInstance.start();
        landInstance.release();
    }
}