using UnityEngine;

[RequireComponent (typeof(PositionFollower))]

public class GunLocomotion : MonoBehaviour
{
    [Header("Sway Properties")]
    public float smooth;
    public float swayMultiplier;

    [Header("Movement Properties")]
    public float effectIntensity;
    public float effectIntensityX;
    public float effectSpeed;
    public float sprintEffectIntensity;
    public float sprintEffectIntensityX;
    public float sprintEffectSpeed;

    private PositionFollower FollowerInstance;
    private Vector3 OriginalOffset;
    private float sinTime;
    private bool isSprinting;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FollowerInstance = GetComponent<PositionFollower>();
        OriginalOffset = FollowerInstance.offset;
    }

    // Update is called once per frame
    void Update()
    {
        GunSway();
        GunMovement();
    }

    public void GunSway()
    {
        // Get Mouse Input

        float mouseX = Input.GetAxisRaw("Mouse X") * swayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMultiplier;

        // Caluculate Target Rotation

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // Rotate

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    public void GunMovement()
    {
        Vector3 inputVector = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));

        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (isSprinting == false)
        {
            if (inputVector.magnitude > 0f)
            {
                sinTime += Time.deltaTime * effectSpeed;
            }
            else
            {
                sinTime = 0f;
            }

            float sinAmountY = -Mathf.Abs(effectIntensity * Mathf.Sin(sinTime));
            Vector3 sinAmount = FollowerInstance.transform.right * effectIntensity * Mathf.Cos(sinTime) * effectIntensityX;

            FollowerInstance.offset = new Vector3
            {
                x = OriginalOffset.x,
                y = OriginalOffset.y + sinAmountY,
                z = OriginalOffset.z
            };
        }
        else if (isSprinting == true)
        {
            if (inputVector.magnitude > 0f)
            {
                sinTime += Time.deltaTime * sprintEffectSpeed;
            }
            else
            {
                sinTime = 0f;
            }

            float sinAmountY = -Mathf.Abs(sprintEffectIntensity * Mathf.Sin(sinTime));
            Vector3 sinAmount = FollowerInstance.transform.right * sprintEffectIntensity * Mathf.Cos(sinTime) * sprintEffectIntensityX;

            FollowerInstance.offset = new Vector3
            {
                x = OriginalOffset.x,
                y = OriginalOffset.y + sinAmountY,
                z = OriginalOffset.z
            };
        }
    }
}
