using Assets.Scripts;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    public Collider enemyCollider;

    [Header("Impact Variables")]
    //public GameObject explosionPosition;
    public float explosionForce;
    public float explosionRadius;
    public float explosionLift;

    public bool ragdollEnabled = false; // Variable to enable updating view bounds because they do not follow the ragdoll/armature leading to weird rendering issues when they are far from the point to where the
    [SerializeField] private SkinnedMeshRenderer mesh;
    private Bounds meshBounds;
    private Rigidbody root;
    private PlayerShooting playerShooting;
    
    //public Rigidbody enemyRigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerShooting = FindFirstObjectByType<PlayerShooting>();
        if (mesh)
        {
            meshBounds = mesh.bounds;
        }
        root = GetComponentInChildren<Rigidbody>();
        EnableRagdoll(ragdollEnabled);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableRagdoll(bool enable)
    {
        SetColliderState(enable);
        SetRigidbodyState(!enable);
        ragdollEnabled = enable;
        if (enable == false)
        {
            UpdateViewBounds();
        }
    }

    void FixedUpdate()
    {
        if (ragdollEnabled)
        {
            UpdateViewBounds();
        }
    }

    private void UpdateViewBounds()
    {
        if (mesh)
        {
            meshBounds = mesh.localBounds;
            if (root)
            {
                meshBounds.center = root.gameObject.transform.localPosition;
                //Debug.Log("Visible Bound Center: " + meshBounds.center);
            }
            //mesh.bounds = meshBounds;
            mesh.localBounds = meshBounds;
        }
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
            if (collider.gameObject.GetComponent<IDamageable>() != null) // Is this a damageable limb, prevent the collider from being disabled if so (or else the limb system doesn't work)
            {
                collider.isTrigger = !state;
                if (!state) continue; // The limb system will handle the collision.
            }
            collider.enabled = state;
            Physics.IgnoreCollision(collider, playerShooting.gameObject.transform.GetComponent<Collider>());
        }
    }

    public void ApplyForceToRagdoll(float Amount = 0)
    {
        if (Amount == 0) Amount = explosionForce;
        else Amount /= 3;

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.AddExplosionForce(Amount, playerShooting.hitPosition, explosionRadius, explosionLift, ForceMode.Impulse);
        }
    }
}
