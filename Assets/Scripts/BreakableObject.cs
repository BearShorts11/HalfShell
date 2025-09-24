using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

// Modified upon tutorial by LlamAcademy: https://youtu.be/3OWeCDr1RUs?si=y8uktvka04yluJHy

public class BreakableObject : MonoBehaviour
{
    private Rigidbody Rigidbody;

    [SerializeField] private GameObject BrokenInstance;

    // Configurable Variables that control the breaking of the object when finally broken
    [SerializeField] public float ExplosiveForce = 10000f;
    [SerializeField] public float ExplosiveRadius = 2f;

    // Variables that fade out broken pieces after destruction
    [SerializeField] private float PieceFadeSpeed = .25f;
    [SerializeField] private float PieceDestroyDelay = 5f;
    [SerializeField] private float PieceSleepCheckDelay = 0.1f;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
    }

    // Ensures the destroyed object doesn't move after being destroyed + destroys the previous object
    public void Break()
    {
        //if (Rigidbody != null)
        //{
        //    Destroy(Rigidbody);
        //}

        // Grabs the collider and renderer from the parent object + any possible children and deactivates them
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        // Places the broken object prefab in exactly the same position as the previous object, then throws the broken pieces
        GameObject brokenInstance = Instantiate(BrokenInstance, transform.position, transform.rotation);

        Rigidbody[] rigidbodies = brokenInstance.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody body in rigidbodies)
        {
            if (Rigidbody != null)
            {
                body.linearVelocity = Rigidbody.linearVelocity;
            }

            body.AddExplosionForce(ExplosiveForce, transform.position, ExplosiveRadius);
        }

        // Calls for the broken pieces to begin fading out
        StartCoroutine(FadeOutRigidBodies(rigidbodies));
    }



    // Slowly moves the broken pieces down and out of the worldspace before permanent deletion
    private IEnumerator FadeOutRigidBodies(Rigidbody[] rigidbodies)
    {
        Debug.Log("FadeOut Called");
        WaitForSeconds wait = new WaitForSeconds(PieceSleepCheckDelay);
        int activeRigidbodies = rigidbodies.Length; 

        while(activeRigidbodies > 0)
        {
            Debug.Log("waiting...");
            yield return wait;

            foreach(Rigidbody body in rigidbodies)
            {
                if (body.IsSleeping())
                {
                    Debug.Log("Eliminating Active Rigidbodies");
                    activeRigidbodies--;
                }
            }
        }

        Debug.Log("waiting...");
        yield return new WaitForSeconds(PieceDestroyDelay);


        float time = 0;
        Renderer[] renderers = Array.ConvertAll(rigidbodies, GetRendererFromRigidbody);
        Debug.Log("Coverted Rigidbodies");

        foreach (Rigidbody body in rigidbodies)
        {
            Debug.Log("Destroyed Rigidbodies");
            Destroy(body.GetComponent<Collider>());
            Destroy(body);
        }

        // This controls the fade out behavior specifically
        // Currently melts the pieces into the ground until they are out of view
        while(time <= 1)
        {
            float step = Time.deltaTime * PieceFadeSpeed;
            foreach(Renderer renderer in renderers)
            {
                Debug.Log("Melting");
                renderer.transform.Translate(Vector3.down * (step / renderer.bounds.size.y), Space.World);
            }

            time += step;
            yield return null;
        }

        foreach(Renderer renderer in renderers)
        {
            Debug.Log("Destroyed Instance");
            Destroy(renderer.gameObject);
            Destroy(renderer);
        }

        // Destroys entire breakable object after co-coutine completes and all destroyed pieces have vanished
        Destroy(gameObject);
    }

    private Renderer GetRendererFromRigidbody(Rigidbody rigidbody)
    {
        return Rigidbody.GetComponent<Renderer>();
    }
}
