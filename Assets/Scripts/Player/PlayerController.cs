using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;

public class PlayerController : MonoBehaviour
{
    //Player
    PlayerBehavior behavior;
    PlayerShooting shooting;
    PlayerCameraInputs cameraInputs;
    [SerializeField] Dialogue dialogue;

    //UI
    ShellWheelController shellWheelController;
    PauseMenu pauseMenu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behavior = GetComponent<PlayerBehavior>();
        shooting = GetComponent<PlayerShooting>();
        cameraInputs = GetComponent<PlayerCameraInputs>();

        shellWheelController = FindFirstObjectByType<ShellWheelController>();
        pauseMenu = FindFirstObjectByType<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMovement(InputAction.CallbackContext context)
    { 
        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (behavior != null) behavior.Jump(context.performed);
    }

    public void OnLook(InputAction.CallbackContext value)
    {
        if(cameraInputs != null) cameraInputs.LookInput(value.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if(behavior != null) behavior.SetIsRunning(context.performed);
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        if (shooting != null && PlayerShooting.canFire && context.performed) { shooting.Fire(); dialogue.EndDialogue(); }
    }

    public void OnRack(InputAction.CallbackContext context)
    {
        if (context.started && shooting != null)
        {
            shooting.PumpBack();
            dialogue.EndDialogue();
        }
        else if (context.canceled && shooting != null)
        {
            shooting.PumpFWD();  
        }
    }

    public void OnShellWheel(InputAction.CallbackContext context)
    {
        if (shellWheelController != null) 
        {
            shellWheelController.PlayerCanOpenWheel();
            shellWheelController.ToggleWheel(context.performed);
            dialogue.EndDialogue();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (pauseMenu != null) { pauseMenu.Pause(); dialogue.EndDialogue(); }
    }

    public void OnAddHS(InputAction.CallbackContext context)
    {
        if (shooting != null && context.started) { shooting.AddHalfShell(); dialogue.EndDialogue(); }
    }

    public void OnAddSL(InputAction.CallbackContext context)
    {
        if (shooting != null && context.started) { shooting.AddSlug(); dialogue.EndDialogue(); }
    }

    public void OnAddFS(InputAction.CallbackContext context)
    {
        if (shooting != null && context.started) { shooting.AddIncindiary(); dialogue.EndDialogue(); }
    }

    public void OnControl(InputAction.CallbackContext context)
    {
        if (dialogue != null && context.performed) { dialogue.InputTrigger(); Debug.Log("here"); }
        
    }
}
