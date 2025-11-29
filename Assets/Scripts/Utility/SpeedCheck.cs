using Unity.VisualScripting;
using UnityEngine;

// Change Collision Detection type based on speed/velocity so that the rigid body does it's damnest to not clip through walls
// Not very usefull if your object is already set to Continuous (or works continuously, only for a short time before the game object is destroyed)
public class SpeedCheck : MonoBehaviour
{
    private Rigidbody rb;

    private CollisionDetectionMode defaultDetection; // Store the initial detection mode 
    private bool fast;
    private bool slow;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        defaultDetection = rb.collisionDetectionMode;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (rb.IsDestroyed()) 
        { 
            Destroy(this);
            return;
        }
        if (rb.IsSleeping()) return;

        fast = (rb.linearVelocity.sqrMagnitude > 100 || rb.angularVelocity.sqrMagnitude > 100);
        slow = !fast;
        if (slow && rb.collisionDetectionMode != defaultDetection)
        {
            rb.collisionDetectionMode = defaultDetection;
            return;
        }
        if (fast && rb.collisionDetectionMode != CollisionDetectionMode.Continuous)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
}
