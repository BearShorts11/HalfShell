using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Windows;
using Unity.Cinemachine;

// Code Stolen Directly From a Unity Tutorial by @ Brogammer on Youtube
// https://www.youtube.com/watch?v=1uW-GbHrtQc
// Edits/additions made to variables and values

[RequireComponent(typeof(CharacterController))]
public class PlayerBehavior : MonoBehaviour
{
    public Camera playerCamera;
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpPower = 10f;
    public float gravity = 20f;
    public float lookSpeed = 3f;
    public float lookXLimit = 90f;
    public float defaultHeight = 2f;

    private float health = 100f;
    public float Health
    {
        get { return health; }
        set
        {
            health = value;

            //this no worky but I want it to -N
            if (health <= 0) { OnDeath(); }
        }
    }

    public GameObject GameOverTxt;

    // Uncomment if Lvl Design feels strongly for crouching -A
    //public float crouchHeight = 1f;
    //public float crouchSpeed = 3f;

    // Every Boomer Shooter needs an always run setting
    public bool alwaysRun = false;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;
    private static bool canLook = true;

    public float SlowedTime = 0.1f;
    public static bool SlowMoActive = false;

    public TextMeshProUGUI HPtext;

    [Header("Cinemachine")]
    public CinemachineCamera playerCam;
    public GameObject CinemachineCameraTarget;
    public PlayerInput input;
    public PlayerCameraInputs cameraInput;
    public float TopClamp = 90.0f;
    public float BottomClamp = -90.0f;
    public float rotationVelocity;

    private bool isCurrentDeviceMouse
    {
        get { return input.currentControlScheme == "KeyBoardMouse"; }
    }
    private float cinemachineTargetPitch;
    private const float threshold = 0.01f;

    //game over sounds
    public EventReference deathRemark;
    // Dedicating a function that just calls this so the code isn't full of these really long function calls -V
    /// <summary>
    /// Plays a sound from the game object that this script is attached to, in this case, the player
    /// </summary>
    /// <param name="eventReference"> The path to the FMOD sound event </param>
    private void PlaySound(EventReference eventReference)
    {
        RuntimeManager.PlayOneShotAttached(eventReference, this.gameObject);
    }
    void Start()
    {
        // Object needs a Character Controller for Script to work
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HPtext.text = $"HP: {health}";
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = UnityEngine.Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * UnityEngine.Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * UnityEngine.Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (alwaysRun)
        {
            walkSpeed = runSpeed;

        }


        // Checks that the player can move and is touching the ground when they press the "Jump" input key, then allows them to jump
        if (UnityEngine.Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }


        // Checks if the player is touching the ground. If not, applies the force of gravity to send them downward
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        #region crouching input (CURRENTLY UNSUED) 
        //keep or delete? -N
        //Keep in case lvl desgn wants crouching for some reason - A

        //// Checks if the player is pressing the crouch key and can move, then sets player to crouch height and overrides sprinting/walking movement speeds
        //if (Input.GetKey(KeyCode.LeftControl) && canMove || Input.GetKey(KeyCode.C) && canMove)
        //{
        //    characterController.height = crouchHeight;
        //    walkSpeed = crouchSpeed;
        //    runSpeed = crouchSpeed;
        //}
        //else
        //{
        //    characterController.height = defaultHeight;
        //    walkSpeed = 6f;
        //    runSpeed = 12f;
        //}
        #endregion


        //why this hanging out in the ether? -N
        // ...I dunno -A
        characterController.Move(moveDirection * Time.deltaTime);


        // Ensures the player doesn't break their neck by looking 360 degrees along the Y axis
        //if (canMove && canLook)
        {
            rotationX += -UnityEngine.Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, UnityEngine.Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (SlowMoActive)
        {
            Time.timeScale = SlowedTime;
        }
        else
        {
            Time.timeScale = 1;
        }

        if (health <= 0)
        {

            
            Time.timeScale = 0;

        }
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    private void CameraRotation()
    {
        // if there is an input
        if (cameraInput.look.sqrMagnitude >= threshold)
        {
            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = 1.0f;
            cinemachineTargetPitch += cameraInput.look.y * lookSpeed * deltaTimeMultiplier;
            rotationVelocity = cameraInput.look.x * lookSpeed * deltaTimeMultiplier;

            // clamp our pitch rotation
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * rotationVelocity);
        }
    }


    public void NoMove() => canMove = false;
    public void YesMove() => canMove = true;
    public static void UnlockCursor()
    {
        canLook = false;
        SlowMoActive = true;
        PlayerShooting.canFire = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public static void LockCursor()
    {
        canLook = true;
        SlowMoActive = false;
        PlayerShooting.canFire = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    //better way to do this? -N
    public void Damage(float damage)
    {
        if (health > 0)
        {
            health -= damage;
            HPtext.text = $"HP: {health}";
        }

        if (health <= 0)
        {

            OnDeath();
            

        }
    }

    private void OnDeath()
    {
        PlaySound(deathRemark);
        NoMove();
        //display game over txt
        GameOverTxt.SetActive(true);
    }

}
