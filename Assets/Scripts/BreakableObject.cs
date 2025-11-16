using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using static UnityEditor.PlayerSettings;

// Modified upon tutorial by LlamAcademy: https://youtu.be/3OWeCDr1RUs?si=y8uktvka04yluJHy

public class BreakableObject : MonoBehaviour
{
    private Rigidbody Rigidbody;
    [SerializeField] private ParticleSystem Particles;
    [SerializeField] public GameObject undamagedPrefab;
    [SerializeField] public GameObject brokenPrefab;
    [SerializeField] public GameObject damagedPrefab;
    [SerializeField] public GameObject debrisPrefab;
    // TO-DO: make particle systems child of prefab object

    private bool isDamaged = false;
    private Vector3 explodePos;
    public Vector3 ExplodePos { set { explodePos = value; } }
    [field: SerializeField] public bool explosionOveride { get; private set; } // determines if the explosion direction should occur from the object instead of the external damage source.


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
        explodePos = this.gameObject.transform.position;

        if (undamagedPrefab != null)    { undamagedPrefab.SetActive(true); }
        if (damagedPrefab != null)      { damagedPrefab.SetActive(false); }
        if (brokenPrefab != null)       { brokenPrefab.SetActive(false); }
        if (debrisPrefab != null)       { debrisPrefab.SetActive(false); }
        // TO-DO: grab particles component on Awake
    }

    public void Damage(float damageAmt)
    {
        currentHealth -= damageAmt;
        Debug.Log("Breakable Item HP: " + currentHealth);

        if (currentHealth <= (maxHealth / 2) && currentHealth > 0 && isDamaged == false && damagedPrefab != null) { Chip(); }
        else if (currentHealth <= 0) { Break(); }
    }


    public void Chip()
    {
        if (Particles != null) { Particles.Play(); }

        undamagedPrefab.SetActive(false);
        damagedPrefab.transform.position = gameObject.transform.position;
        damagedPrefab.transform.rotation = gameObject.transform.rotation;
        damagedPrefab.SetActive(true);
        Explode(damagedPrefab);

        isDamaged = true;
    }


    // Ensures the destroyed object doesn't move after being destroyed + destroys the previous object
    public void Break()
    {
        Destroy(GetComponent<Collider>());

        if (Particles != null) { Particles.Play(); }

        if (undamagedPrefab != null)    { undamagedPrefab.SetActive(false); }
        if (damagedPrefab != null)      { damagedPrefab.SetActive(false); }

        if (brokenPrefab != null)
        {
            brokenPrefab.transform.position = gameObject.transform.position;
            brokenPrefab.transform.rotation = gameObject.transform.rotation;
            brokenPrefab.SetActive(true);
            Explode(brokenPrefab);
        }
        if (debrisPrefab != null)
        {
            debrisPrefab.transform.position = gameObject.transform.position;
            debrisPrefab.transform.rotation = gameObject.transform.rotation;
            debrisPrefab.SetActive(true);
            debrisPrefab.transform.parent = null;
        }
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
        if (brokenPrefab.activeSelf == true) { Destroy(gameObject); }
    }

    // Exerts a force on any rigidbodies in the subject prefab
    private void Explode(GameObject prefab)
    {
        Rigidbody[] rigidbodies = prefab.GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody body in rigidbodies)
        {
            if (Rigidbody != null) { body.linearVelocity = Rigidbody.linearVelocity; }
            body.AddExplosionForce(explosiveForce, explodePos, explosiveRadius);
        }

        // Calls for the broken pieces to begin fading out
        StartCoroutine(FadeOutRigidBodies(rigidbodies));
    }

    private Renderer GetRendererFromRigidbody(Rigidbody Rigidbody) { return Rigidbody.GetComponent<Renderer>(); }
}
