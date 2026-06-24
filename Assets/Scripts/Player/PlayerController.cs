using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //Player
    PlayerBehavior behavior;
    PlayerShooting shooting;
    PlayerCameraInputs cameraInputs;

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
        if (shooting != null && PlayerShooting.canFire && context.performed) { shooting.Fire(); }
    }

    public void OnRack(InputAction.CallbackContext context)
    {
        if (context.started && shooting != null)
        {
            shooting.PumpBack();
        }
        else if (context.canceled && shooting != null)
        {
            shooting.PumpFWD();  
        }
    }

    public void OnShellWheel(InputAction.CallbackContext context)
    {
        if (shellWheelController != null) { shellWheelController.ToggleWheel(context.performed); }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (pauseMenu != null) { pauseMenu.Pause(); }
    }

    public void OnAddHS(InputAction.CallbackContext context)
    {
        if (shooting != null && context.started) shooting.AddHalfShell();
    }

    public void OnAddSL(InputAction.CallbackContext context)
    {
        if (shooting != null && context.started) shooting.AddSlug();
    }

    public void OnAddFS(InputAction.CallbackContext context)
    {
        if (shooting != null && context.started) shooting.AddIncindiary();
    }
}
