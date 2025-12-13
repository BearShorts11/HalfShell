using UnityEngine;

// Downforce of the Game Object, making it fall faster.
public class Weight : MonoBehaviour
{
    [Header("How much the object weighs, making them fall down faster.")]
    public float weight = 10f;
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (rb.IsSleeping() || Mathf.Abs(rb.linearVelocity.y) <= 1f) return;

        rb.AddForce(Vector3.down * weight);
    }
}
