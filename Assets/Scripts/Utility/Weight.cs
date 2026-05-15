using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

// Downforce of the Game Object, making it fall faster. Also does sounds
public class Weight : MonoBehaviour
{
    [Header("How much the object weighs, making them fall down faster.\n0: Disabled\n>1:Heavier\n<0 to -1:Lighter")]
    public float weight = 0f;
    public EventReference physicsSound;
    private Rigidbody rb;
    SimpleSoundEvent physicsEventSound;

    void Awake()
    {
        physicsEventSound = GetComponent<SimpleSoundEvent>();
        if (physicsEventSound is null)
            physicsEventSound = this.AddComponent<SimpleSoundEvent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (rb == null) return;

        if (collision.relativeVelocity.sqrMagnitude > 5f)
        {
            physicsEventSound.PlayInstance(physicsSound);
            physicsEventSound.ChangeInstanceParameter("ImpactIntensity " + (collision.relativeVelocity.sqrMagnitude / 100));
            physicsEventSound.ReleaseEventInstance();
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        if (rb.IsSleeping() || Mathf.Abs(rb.linearVelocity.sqrMagnitude) <= 0.1f) return;

        rb.AddForce(Vector3.down * (weight / Physics.gravity.sqrMagnitude));
    }
}
