using System.Collections;
using System.Collections.Generic;
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

    // No Croutching intended for Half Shell Prototype
    //public float crouchHeight = 1f;
    //public float crouchSpeed = 3f;

    // Every Boomer Shooter needs an always run setting
    public bool alwaysRun = false;

    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;
    

    void Start()
    {
        // Object needs a Character Controller for Script to work
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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


        //why this hanging out in the ether? -N
        characterController.Move(moveDirection * Time.deltaTime);


        // Ensures the player doesn't break their neck by looking 360 degrees along the Y axis
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    public void NoMove() => canMove = false;
    public void YesMove() => canMove = true;
}
