using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Collider enemyCollider;

    [Header("Impact Variables")]
    //public GameObject explosionPosition;
    public float explosionForce;
    public float explosionRadius;
    public float explosionLift;

    private PlayerShooting playerShooting;
    
    //public Rigidbody enemyRigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerShooting = FindFirstObjectByType<PlayerShooting>();
        SetRigidbodyState(true);
        SetColliderState(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetRigidbodyState(bool state)
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        
    }

    public void SetColliderState(bool state)
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        enemyCollider.enabled = !state;

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }
    }

    public void ApplyForceToRagdoll()
    {
        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.AddExplosionForce(explosionForce, playerShooting.hitPosition, explosionRadius, explosionLift, ForceMode.Impulse);
        }
    }
}
