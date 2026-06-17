using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerBehavior behavior;
    PlayerShooting shooting;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        behavior = GetComponent<PlayerBehavior>();
        shooting = GetComponent<PlayerShooting>();
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
    
    }

    public void OnSprint(InputAction.CallbackContext context)
    { 
    
    }

    public void OnShoot(InputAction.CallbackContext context)
    { 
        
    }

    public void OnRack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            shooting.PumpBack();
        }
        else if (context.canceled)
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
