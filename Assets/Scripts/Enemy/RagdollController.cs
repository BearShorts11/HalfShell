using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Collider enemyCollider;

    [Header("Impact Variables")]
    public GameObject explosionPosition;
    public float explosionForce;
    public float explosionRadius;
    
    //public Rigidbody enemyRigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
            rigidbody.AddExplosionForce(explosionForce, explosionPosition.transform.position, explosionRadius, 0.0f, ForceMode.Impulse);
        }
    }
}
