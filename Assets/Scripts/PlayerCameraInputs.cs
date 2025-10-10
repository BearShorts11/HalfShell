using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraInputs : MonoBehaviour
{

    [Header("Camera Input Values")]
    public Vector2 look;
    public bool cursorInputForLook = true;

    public void OnLook(InputValue value)
    {
        if(cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }
}
