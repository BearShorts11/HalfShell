using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerBehavior behavior;
    PlayerShooting shooting;
    PlayerCameraInputs cameraInputs;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behavior = GetComponent<PlayerBehavior>();
        shooting = GetComponent<PlayerShooting>();
        cameraInputs = GetComponent<PlayerCameraInputs>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMovement(InputAction.CallbackContext context)
    { 
    
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if(cameraInputs != null) cameraInputs.LookInput(context.ReadValue<Vector2>());
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if(behavior != null) behavior.SetIsRunning(context.performed);
    }

    public void OnShoot(InputAction.CallbackContext context)
    { 
        
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
        
    }

    public void OnPause(InputAction.CallbackContext context)
    { 
        
    }
}
