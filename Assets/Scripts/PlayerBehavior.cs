using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    

    void Start()
    {
        // Object needs a Character Controller for Script to work
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        HPtext.text = $"HP: {health}";
    }


    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (alwaysRun)
        {
            walkSpeed = runSpeed;
        }


        // Checks that the player can move and is touching the ground when they press the "Jump" input key, then allows them to jump
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
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
        if (canMove && canLook)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }

        if (SlowMoActive)
        {
            Time.timeScale = SlowedTime;
        }
        else
        {
            Time.timeScale = 1;
        }



        #region UI Controls

        // To be added revised soon
        // Shell Wheel UI
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{

        //    ShellWheelController.shellWheelSelected = true;
        //}
        //if (Input.GetKeyUp(KeyCode.Tab))
        //{
        //    ShellWheelController.shellWheelSelected = false;
        //}
        #endregion

        if (Input.GetKeyDown(KeyCode.F))
        {
            BlowUp();
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
        NoMove();
        //display game over txt
        GameOverTxt.SetActive(true);
    }



    // TEMPORARY FUNCTIONALITY TO TEST DESTRUCTIBLE OBJECTS
    //private LayerMask interactableLayers;
    private float useDistance = 3f;
    [SerializeField] private Camera camera;
    private void BlowUp()
    {
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out RaycastHit hit, useDistance) && hit.collider.TryGetComponent<BreakableObject>(out BreakableObject breakable))
        {
            breakable.Break();
        }
    }
}
