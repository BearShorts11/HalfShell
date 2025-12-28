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
using Assets.Scripts;
using System;
using Unity.VisualScripting;

// Code Stolen Directly From a Unity Tutorial by @ Brogammer on Youtube
// https://www.youtube.com/watch?v=1uW-GbHrtQc
// Edits/additions made to variables and values
// Past Spetember 2025, the above statement is hardly true. -A

[RequireComponent(typeof(CharacterController))]
public class PlayerBehavior : MonoBehaviour, IDamageable
{
    public Camera playerCamera;
    public float walkSpeed = 10f;
    public float runSpeed = 15f;
    public float jumpPower = 5f;
    public float gravity = 15f;
    public float lookSpeed = 3f;
    public float lookXLimit = 90f;
    public float defaultHeight = 2f;
    public bool startWithShotgun;

    private float sensitivityModifier = 1f;
    public const float DEFAULT_SENSITIVITY_MOD = 1f;
    public const string SENSITIVITY_KEY = "SENSITIVITY";
    private float FOVvalue = 60f;
    public const float DEFAULT_FOV_VALUE = 60f;
    public const string FOV_KEY = "FOV";

    [SerializeField] private bool invincible;
    [SerializeField] private float health = 100f;
    public float maxHealth { get { return 100f; } }
    public float Health
    {
        get { return health; }
        set
        {
            health = value;

            if (health > maxHealth) health = maxHealth;

            //this no worky but I want it to -N
            if (health <= 0) { OnDeath(); }
        }
    }

    [SerializeField] private float armor = 0f;
    private float maxArmor = 100f;
    public float Armor
    {
        get { return armor; }
        set
        {
            armor = value;
            if (armor > maxArmor) armor = maxArmor;
            if (armor < 0) armor = 0;
        }
    }
    public float MaxArmor
    {
        get { return maxArmor; }
    }

    // Uncomment if Lvl Design feels strongly for crouching -A
    //public float crouchHeight = 1f;
    //public float crouchSpeed = 3f;

    [SerializeField] private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;

    private bool canMove = true;
    private static bool canLook = true;

    public float SlowedTime = 0.1f;
    public static bool SlowMoActive = false;

    private PlayerUI UI;

    [Header("Cinemachine")]
    public CinemachineCamera playerCinemachineCamera;
    public GameObject CinemachineCameraTarget;
    public PlayerInput input;
    public PlayerCameraInputs cameraInput;
    public float TopClamp = 90.0f;
    public float BottomClamp = -90.0f;
    public float rotationVelocity;

    private PlayerShooting playerShooting;
    public GameObject ShotgunViewmodel;
    public GameObject ApollyonBark;

    private bool isCurrentDeviceMouse
    {
        get { return input.currentControlScheme == "KeyBoardMouse"; }
    }
    private float cinemachineTargetPitch;
    private const float threshold = 0.01f;

    //game over sounds
    //public EventReference deathRemark;
    public EventReference dmgEfforts;
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
        input = GetComponent<PlayerInput>();
        cameraInput = GetComponent<PlayerCameraInputs>();

        UI = FindFirstObjectByType<PlayerUI>();
        playerShooting = FindFirstObjectByType<PlayerShooting>();
        LockCursor();
        ResumeTime();
        UI.UpdateHP(health, maxHealth);
        UI.UpdateMaxHP(maxHealth, maxHealth);
        UI.UpdateArmor(armor, maxArmor);
        UI.UpdateMaxArmor(armor, maxArmor);

        if (!startWithShotgun)
        {
            playerShooting.enabled = false;
            ShotgunViewmodel.SetActive(false);
            ApollyonBark.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    void Update()
    {
        if (PauseMenu.paused == true)
        {
            return;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = UnityEngine.Input.GetKey(KeyCode.LeftShift);
        //bool isRunning = false;

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * UnityEngine.Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * UnityEngine.Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (isRunning) { moveDirection = Vector3.ClampMagnitude(moveDirection, runSpeed); }
        else { moveDirection = Vector3.ClampMagnitude(moveDirection, walkSpeed); }


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
        if (!characterController.isGrounded && movementDirectionY > -15) //-15 ensures the player doens't keep adding downward velocity forever.
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


        characterController.Move(moveDirection * Time.deltaTime);


        // Ensures the player doesn't break their neck by looking 360 degrees along the Y axis
        if (canMove && canLook)
        {
            rotationX += -UnityEngine.Input.GetAxis("Mouse Y") * lookSpeed * sensitivityModifier;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, UnityEngine.Input.GetAxis("Mouse X") * lookSpeed * sensitivityModifier, 0);
        }

        if (SlowMoActive)
        {
            Time.timeScale = SlowedTime;
        }
        else
        {
            Time.timeScale = 1;
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
        if (cameraInput.look.sqrMagnitude >= threshold && canLook)
        {
            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = 1.0f;
            cinemachineTargetPitch += cameraInput.look.y * lookSpeed * sensitivityModifier * deltaTimeMultiplier;
            rotationVelocity = cameraInput.look.x * lookSpeed * sensitivityModifier * deltaTimeMultiplier;

            // clamp our pitch rotation
            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * rotationVelocity);
        }
    }

    public void UpdateSensitivity()
    {
        if (PlayerPrefs.HasKey(SENSITIVITY_KEY)) sensitivityModifier = PlayerPrefs.GetFloat(SENSITIVITY_KEY);
    }
    public void UpdateFOV()
    {
        //for some reason does not do exact value, but still works fine
        if (PlayerPrefs.HasKey(FOV_KEY)) playerCinemachineCamera.Lens.FieldOfView = PlayerPrefs.GetFloat(FOV_KEY);
    }

    public void NoMove() => canMove = false;
    public void YesMove() => canMove = true;
    public static void UnlockCursor()
    {
        canLook = false;
        PlayerShooting.canFire = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public static void LockCursor()
    {
        canLook = true;
        PlayerShooting.canFire = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void PauseTime() => Time.timeScale = 0;
    public static void ResumeTime() => Time.timeScale = 1;


    //better way to do this? -N
    public void Damage(float damage)
    {
        if (invincible) return;
        {
            
        }
        if (health > 0)
        {
            if (armor > 0) 
            { 
                float newDamage = (damage / 3);
                if (armor < (newDamage * 2))
                {
                    float spillover = (newDamage * 2) - armor;
                    Armor = 0;
                    newDamage = spillover;
                }
                else { Armor -= (newDamage * 2); }
                //Armor -= Mathf.Round(newDamage * 2);
                UI.UpdateArmor(armor, maxArmor);
                damage = newDamage;
            }
            health -= damage;
            UI.UpdateHP(health, maxHealth);
            PlaySound(dmgEfforts);
        }

        if (health <= 0)
        {
            OnDeath();
        }

        UI.Hurt();
    }

    public void GainHealth(float gainhealth)
    {
        health = health + gainhealth;
    }


    public void OnDeath()
    {
        //PlaySound(deathRemark);
        NoMove();

        //display game over txt
    }

    public void SetHealth(float health)
    {
        this.health = health;
        if (UI is null) UI = FindFirstObjectByType<PlayerUI>();
        UI.UpdateHP(health, maxHealth);

        if (UI.player is null) UI.player = this;

        UI.CheckHealth();
    }
    public void SetArmor(float armor)
    { 
        this.armor = armor;
    }

    public void EquipShotgun()
    {
        playerShooting.enabled = true;
        ShotgunViewmodel.SetActive(true);
        ApollyonBark.SetActive(true) ;
    }

    public void Invincible() => invincible = true;
    public void Mortal() => invincible = false;

}
