using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

// Modified upon tutorial by LlamAcademy: https://youtu.be/3OWeCDr1RUs?si=y8uktvka04yluJHy

public class BreakableObject : MonoBehaviour
{
    private Rigidbody Rigidbody;
    [SerializeField] private ParticleSystem Particles;
    [SerializeField] public GameObject brokenPrefab;
    [SerializeField] public GameObject damagedPrefab;
    [SerializeField] public GameObject debrisPrefab;
    // TO-DO: make particle systems child of prefab object

    [SerializeField] public float maxHealth = 100;
    [SerializeField] public float currentHealth;
    // TO-DO: add resistance variable when dmg types are added

    [SerializeField] private float explosiveForce = 1000;
    [SerializeField] private float explosiveRadius = 2;

    [SerializeField] private float pieceFadeSpeed = 0.25f;
    [SerializeField] private float pieceShrinkMult = 1;
    [SerializeField] private float pieceDestroyDelay = 5f;
    [SerializeField] private float pieceSleepCheckDelay = 0.1f;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        currentHealth = maxHealth;
        // TO-DO: grab particles component on Awake
    }


    public void Chip()
    {
        Destroy(Rigidbody);
        GetComponent<Renderer>().enabled = false;

        Particles.Play();

        Renderer[] oldRenderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer oldRenderer in oldRenderers)
        {
            oldRenderer.enabled = false;
        }

        // TO-DO: make damaged prefab child object and activate, DON'T instantiate
        GameObject damagedInstance = Instantiate(damagedPrefab, transform.position, transform.rotation);
        damagedInstance.transform.parent = transform;
    }


    // Ensures the destroyed object doesn't move after being destroyed + destroys the previous object
    public void Break()
    {
        Particles.Play();

        if (transform.childCount > 0)
        {
            foreach (Transform child in transform.GetChild(0))
            {
                Destroy(child.gameObject);
            }
        }

        GetComponent<Collider>().enabled = false;
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }

        GameObject debrisInstance = Instantiate(debrisPrefab, transform.position, transform.rotation);

        // Instantiates and replaces the previous model/mesh
        // TO-DO: make destroyed prefab child object and activate, DON'T instantiate
        GameObject brokenInstance = Instantiate(brokenPrefab, transform.position, transform.rotation);
        brokenInstance.transform.parent = transform;


        Rigidbody[] rigidbodies = brokenInstance.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody body in rigidbodies)
        {
            if (Rigidbody != null)
            {
                body.linearVelocity = Rigidbody.linearVelocity;
            }
            body.AddExplosionForce(explosiveForce, transform.position, explosiveRadius);
        }

        // Calls for the broken pieces to begin fading out
        StartCoroutine(FadeOutRigidBodies(rigidbodies));
    }



    // Slowly moves the broken pieces down and out of the worldspace before permanent deletion
    private IEnumerator FadeOutRigidBodies(Rigidbody[] Rigidbodies)
    {
        WaitForSeconds Wait = new WaitForSeconds(pieceSleepCheckDelay);
        float activeRigidbodies = Rigidbodies.Length;

        while (activeRigidbodies > 0)
        {
            yield return Wait;

            foreach (Rigidbody rigidbody in Rigidbodies)
            {
                if (rigidbody.IsSleeping())
                {
                    activeRigidbodies--;
                }
            }
        }


        yield return new WaitForSeconds(pieceDestroyDelay);

        float time = 0;
        Renderer[] renderers = Array.ConvertAll(Rigidbodies, GetRendererFromRigidbody);

        foreach (Rigidbody body in Rigidbodies)
        {
            Destroy(body.GetComponent<Collider>());
            Destroy(body);
        }

        // This controls the fade out behavior specifically
        // Currently melts the pieces into the ground until they are out of view
        while (time < 1)
        {
            float step = Time.deltaTime * pieceFadeSpeed;
            foreach (Renderer renderer in renderers)
            {
                renderer.transform.Translate(Vector3.down * (step / renderer.bounds.size.y), Space.World);
                renderer.transform.localScale = renderer.transform.localScale - new Vector3(pieceShrinkMult, pieceShrinkMult, pieceShrinkMult) * step;
            }

            time += step;
            yield return null;
        }

        foreach (Renderer renderer in renderers)
        {
            Destroy(renderer.gameObject);
        }


        // Destroys entire breakable object after co-coutine completes and all destroyed pieces have vanished
        Destroy(Particles.gameObject);
        Destroy(gameObject);
    }



    private Renderer GetRendererFromRigidbody(Rigidbody Rigidbody)
    {
        return Rigidbody.GetComponent<Renderer>();
    }
}
