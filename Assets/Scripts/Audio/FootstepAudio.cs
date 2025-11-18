using UnityEngine;
using FMODUnity;
using FMOD.Studio;
public class PlayerFootstepAudio : MonoBehaviour
{//FMOD EVENTS
    [Header("FMOD Events")]
    [SerializeField] private EventReference footstepEvent;
    [SerializeField] private EventReference landEvent;
    [SerializeField] private string surfaceParameterName = "SurfaceTerrain";
    // ===== SETTINGS =====
    [Header("Settings")]
    [SerializeField] private float stepInterval = 0.5f; // Time between footsteps while moving
    [SerializeField] private float minMoveSpeed = 0.1f;// Minimum player movement speed required to trigger footsteps
    [SerializeField] private float raycastDistance = 1.2f;// How far below the player to check for ground
    [SerializeField] private LayerMask groundMask; // Which layers count as ground for surface detection

    private CharacterController controller;// Reference to the player’s CharacterController
    private Vector3 previousPosition;// Used to measure player movement between frames
    private float stepTimer;// Counts down between footsteps
    private bool wasGrounded;// Tracks if the player was grounded in the previous frame
    private int currentSurfaceIndex;// Stores the current detected surface type (FMOD parameter value)

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (controller == null)
            //  Debug.LogError("<color=red>No CharacterController found!</color>");

            if (!footstepEvent.IsNull) ;
            // Debug.Log("<color=green>Footstep event assigned.</color>");
            else
        // Debug.LogError("<color=red>Footstep event is NOT assigned!</color>");

        if (!landEvent.IsNull) ;
            //  Debug.Log("<color=green>Landing event assigned.</color>");
            else
                //  Debug.LogError("<color=red>Landing event is NOT assigned!</color>");

                previousPosition = transform.position;
        stepTimer = stepInterval;

        //Debug.Log("<color=yellow>Footstep system initialized.</color>");
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;
        float speed = Vector3.Distance(transform.position, previousPosition) / Time.deltaTime;

        // Debug.Log($"Grounded: {isGrounded}, Speed: {speed:F2}, Timer: {stepTimer:F2}");

        DetectSurface();// Detect which surface the player is on

        // Landing
        if (isGrounded && !wasGrounded)
        {
            // Debug.Log("<color=cyan>Landing detected!</color>");
            PlayLandingSound();
        }

        // Footsteps
        if (isGrounded && speed > minMoveSpeed && IsPlayerWalking())
        {
            stepTimer -= Time.deltaTime;

           // Debug.Log($"Footstep countdown: {stepTimer:F2}");

            if (stepTimer <= 0f)
            {
               // Debug.Log("<color=green>Playing FOOTSTEP sound.</color>");
                PlayFootstepSound();
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
    // ===== INPUT CHECK =====
    private bool IsPlayerWalking()
    {// Detect movement input (WASD / Arrow keys)
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        // Player is "walking" if there's movement input
        bool walking = inputX != 0f || inputZ != 0f;

          //Debug.Log($"Input: X={inputX}, Z={inputZ}, Walking={walking}");

        return walking;
    }
    // ===== SURFACE DETECTION =====
    private void DetectSurface()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit hit, raycastDistance, groundMask))
        {
            string tag = hit.collider.tag;
            // Debug.Log($"Surface detected: {tag}");
            // Assign an index based on tag (used by FMOD parameter)
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
           // Debug.LogWarning("<color=orange>No ground hit detected! Check groundMask or raycast distance.</color>");
        }
    }

    private void PlayFootstepSound()
    {
        // Debug.Log($"Sending FMOD Footstep — SurfaceIndex = {currentSurfaceIndex}");

        EventInstance stepInstance = RuntimeManager.CreateInstance(footstepEvent);
        stepInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        stepInstance.setParameterByName(surfaceParameterName, (float)currentSurfaceIndex);
        stepInstance.start();
        stepInstance.release();
    }

    private void PlayLandingSound()
    {
        // Debug.Log($"Sending FMOD Landing — SurfaceIndex = {currentSurfaceIndex}");

        EventInstance landInstance = RuntimeManager.CreateInstance(landEvent);
        landInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform));
        landInstance.setParameterByName(surfaceParameterName, (float)currentSurfaceIndex);
        landInstance.start();
        landInstance.release();
    }
}
