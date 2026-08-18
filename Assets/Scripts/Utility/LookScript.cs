using UnityEngine;

// Look at object script (Following this tutorial: https://www.youtube.com/watch?v=dnFTT5vIb68)
public class LookScript : MonoBehaviour
{
    [Header("Inscribed")]
    public Transform headBone, headFwd; //headFwd should be set to a copy of the headBone object and must be parented to the same object the original is parented to. This is to store the original transform information for the original headBone to reset to.
    [Tooltip("Absolute maximum angle that the head may turn to")]
    [Range(0f, 180f)] public float maxAngle = 70f;
    [Tooltip("Speed at which the head would turn to a specified focus point")]
    public float defaultLookSpeed = 5f;
    [Tooltip("Time which will set the headBone back to it's original resting rotation when the time hits 0")]
    public float headResetTimer = 0.5f;

    [Header("Dynamic")]
    [SerializeField] private bool lookEnabled = false;
    [SerializeField] private float lookSpeed;
    [SerializeField] private float headResetTime;
    [SerializeField] private Transform focusPoint;
    [SerializeField] private bool isLooking = false;
    [SerializeField] private Quaternion lastRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lookSpeed = defaultLookSpeed;

        // If a copy of the headbone object is not made and parented to the object the original headbone is parented to manually, make a copy automatically.
        if (headFwd == null && headBone != null)
        { 
            //headFwd = this.gameObject.transform;
            GameObject newFwd = new GameObject(headBone.gameObject.name + "_FWD");
            newFwd.transform.position = headBone.transform.position;
            newFwd.transform.rotation = headBone.transform.rotation;
            newFwd.transform.parent = headBone.transform.parent;
            headFwd = newFwd.transform;
        }
    }

    void OnValidate()
    {
        if (defaultLookSpeed != lookSpeed)
        {
            lookSpeed = defaultLookSpeed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if (lookEnabled)
            LookAtPoint(focusPoint);
    }

#region For controlling via external components
    public void EnableLooking()
    {
        lookEnabled = true;
        if (IsInvoking(nameof(StopLooking)))
            CancelInvoke(nameof(StopLooking));
    }

    public void DisableLooking()
    {
        LookAtPoint(null);
        Invoke(nameof(StopLooking), headResetTimer);
    }   
    private void StopLooking()
    {
        lookEnabled = false;
    }

    public void SetFocusPoint(Transform point)
    {
        focusPoint = point;
    }

    public void SetLookSpeed(float speed)
    {
        lookSpeed = speed;
    }

    public void ResetLookSpeed()
    {
        lookSpeed = defaultLookSpeed;
    }
# endregion

    public void LookAtPoint(Transform point)
    {
        if (focusPoint != point) focusPoint = point;

        if (focusPoint != null)
        {
            Vector3 Dir = (focusPoint.position - headBone.position).normalized;
            float Angle = Vector3.SignedAngle(Dir, headFwd.forward, headFwd.up);
            //Debug.Log("Look Angle:" + Angle);
            //Debug.Log("Direction normalized:" + Dir);
            //Debug.Log("Focal Point Pos:" + focusPoint.position);
            if ((Mathf.Abs(Angle)) < maxAngle)
            {
                if (!isLooking)
                {
                    isLooking = true;
                    lastRotation = headBone.rotation;
                }
                Quaternion TargetRot = Quaternion.LookRotation(focusPoint.position - headBone.position);
                lastRotation = Quaternion.Slerp(lastRotation, TargetRot, lookSpeed * Time.deltaTime);
                //Debug.Log("Target Rot:" + TargetRot.eulerAngles);

                headBone.rotation = lastRotation;
                headResetTime = headResetTimer;
            }
        }
        else if (isLooking)
        {
            lastRotation = Quaternion.Slerp(lastRotation, headFwd.rotation, lookSpeed * Time.deltaTime);
            headBone.rotation = lastRotation;
            headResetTime -= Time.deltaTime;
            if (headResetTime <= 0)
            {
                headBone.rotation = headFwd.rotation;
                isLooking = false;
            }
        }
    }
}
