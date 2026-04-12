using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Ignores specific game objects in the case that filtering collision through layers is overkill or you want to filter by tags.
public class IgnoreCollision : MonoBehaviour
{
    public enum ColliderType
    {
        Any = 0,
        Box = 1,
        Sphere = 2,
        Capsule = 3,
        Mesh = 4
    }

    public ColliderType colliderType = ColliderType.Any;
    private Collider thisCollider;
    [Tooltip("Filter collisions for specific gameobjects (and pray they do not have any child objects with collision components to them)")]
    public Collider[] colliders = new Collider[new()];
    [Tooltip("Filter collisions by Tag (Both objects must have this component)")]
    public string[] colliderTags = new string[new()];

    private IgnoreCollision Collided = null;
    private float checkRate = 0.5f;
    private float lastCheckTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        switch(colliderType)
        {
            case ColliderType.Any:
                thisCollider = GetComponent<Collider>();
                break;
            case ColliderType.Box:
                thisCollider = GetComponent<BoxCollider>();
                break;
            case ColliderType.Sphere:
                thisCollider = GetComponent<SphereCollider>();
                break;
            case ColliderType.Capsule:
                thisCollider = GetComponent<CapsuleCollider>();
                break;
            case ColliderType.Mesh:
                thisCollider = GetComponent<MeshCollider>();
                break;
        }
    }
    void Start()
    {
        SetUpCollisions();
    }

    void SetUpCollisions()
    {
        if (colliders.Length > 0)
            for (int i = 0; i < colliders.Length; i++)
                Physics.IgnoreCollision(thisCollider, colliders[i]);

        if (colliderTags.Length == 0) return;
        List<Collider> GO_Colliders = new();
        for (int i = 0; i < colliderTags.Length; i++)
        { 
            GameObject[] GOs = GameObject.FindGameObjectsWithTag(colliderTags[i]);
            for (int j = 0; j < GOs.Length; j++)
            {
                GO_Colliders = GOs[j].GetComponents<Collider>().ToList<Collider>();
                foreach(Collider c in GO_Colliders)
                {
                    if (!c.isTrigger)
                    {
                        Physics.IgnoreCollision(thisCollider, c);
                    }
                }
                GO_Colliders.Clear();
            }
        }

    }

    public void CheckCollision(Collision collided)
    {
        if (colliderTags.Length == 0) return;

        if (!Physics.GetIgnoreCollision(thisCollider, collided.collider))
        {
            for (int i = 0; i < colliderTags.Length; i++)
            {
                if (collided.transform.gameObject.CompareTag(colliderTags[i]))
                {
                    Physics.IgnoreCollision(thisCollider, collided.collider, true);
                    break;
                }
            }
        }
    }
    public void CheckCollision(Collider collided)
    {
        if (colliderTags.Length == 0) return;

        if (!Physics.GetIgnoreCollision(thisCollider, collided))
        {
            for (int i = 0; i < colliderTags.Length; i++)
            {
                if (collided.transform.gameObject.CompareTag(colliderTags[i]))
                {
                    Physics.IgnoreCollision(thisCollider, collided, true);
                    break;
                }
            }
        }
    }

    // For the Player...
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (Time.time < lastCheckTime + checkRate) return;

        if (!Physics.GetIgnoreCollision(thisCollider, hit.collider))
        {
            if (hit.collider.transform.gameObject.TryGetComponent<IgnoreCollision>(out Collided))
            {
                Collided.CheckCollision(thisCollider);
            }
        }
        lastCheckTime = Time.time;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.TryGetComponent<IgnoreCollision>(out Collided))
        {
            Collided.CheckCollision(thisCollider);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (colliderTags.Length == 0) return;

        if (!Physics.GetIgnoreCollision(thisCollider, other))
        {
            for (int i = 0; i < colliderTags.Length; i++)
            {
                if (other.transform.gameObject.CompareTag(colliderTags[i]))
                {
                    if (!other.isTrigger)
                        Physics.IgnoreCollision(thisCollider, other);
                    break;
                }
            }
        }
    }


}
