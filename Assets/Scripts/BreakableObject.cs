using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor;
using Unity.VisualScripting;
using Assets.Scripts;
using FMODUnity;

// Modified upon tutorial by LlamAcademy: https://youtu.be/3OWeCDr1RUs?si=y8uktvka04yluJHy

public class BreakableObject : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [field:SerializeField] public float maxHealth { get; private set; }
    [field: SerializeField] public float Health { get; set; }
    // TO-DO: add resistance variable when dmg types are added

    [Header("Damage State Objects")]
    [SerializeField] private ParticleSystem Particles;
    [SerializeField] public GameObject undamagedPrefab;
    [SerializeField] public GameObject brokenPrefab;
    [SerializeField] public GameObject damagedPrefab;
    [SerializeField] public GameObject debrisPrefab;
    // TO-DO: make particle systems child of prefab object
    private Rigidbody Rigidbody;
    private bool isDamaged = false;
    private bool isBroken = false;

    [Header("Destruction Settings")]
    public bool destructionOveride = false;
    private Vector3 destructionPos;
    public Vector3 DestructionPos { set { destructionPos = value; } }
    [SerializeField] private float destructionForce = 1000;
    [SerializeField] private float destructionRadius = 2;

    [Header("Fragment Despawn Settings")]
    [SerializeField] private float pieceFadeSpeed = 0.25f;
    [SerializeField] private float pieceShrinkMult = 1;
    [SerializeField] private float pieceDestroyDelay = 5f;
    [SerializeField] private float pieceSleepCheckDelay = 0.1f;

    [Header("Explosive Object Settings")]
    [SerializeField] public bool explosive;
    [SerializeField] private int fragments = 25;
    private Collider[] fragmentHits;
    [SerializeField] private float explosionRadius = 10f;

    [SerializeField] private LayerMask damageLayer;
    [SerializeField] private LayerMask blockFragmentsLayer;

    [SerializeField] public float maxDamage = 100;
    [SerializeField] public float minDamage = 10;
    [SerializeField] private float explosionForce = 100;

    [SerializeField] private ParticleSystem explosionParticles;
    [SerializeField] private EventReference explosionSound;

    private bool EXPLODED = false;
    // THE MOST IMPORTANT VARIBALE MAKE SURE THIS VARIABLE IS TRUE WHEN AN OBJECT BLOWS UP HOLY FUCK
    // MAKE SURE THIS VARIABLE IS TRUE PLEASE FOR THE LOVE OF GOD
    // YOU WILL BRICK YOUR COMPUTER AND THE ENTIRE PROJECT IF IT IS FALSE WHEN AN OBJECT EXPLODES
    // THAT'S WHY ITS IN ALL CAPS


    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Health = maxHealth;
        destructionPos = this.gameObject.transform.position;

        if (undamagedPrefab != null)    { undamagedPrefab.SetActive(true); }
        if (damagedPrefab != null)      { damagedPrefab.SetActive(false); }
        if (brokenPrefab != null)       { brokenPrefab.SetActive(false); }
        if (debrisPrefab != null)       { debrisPrefab.SetActive(false); }
        // TO-DO: grab particles component on Awake

        if (explosive) { fragmentHits = new Collider[fragments]; }
    }

    public void Damage(float damageAmt)
    {
        Health -= damageAmt;

        if (Health <= (maxHealth / 2) && Health > 0 && isDamaged == false && damagedPrefab != null) { Chip(); }
        else if (Health <= 0) { Break(); }
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
        if (isBroken) return;

        isBroken = true;
        Destroy(GetComponent<Collider>());

        if (Particles != null) { Particles.Play(); }

        if (undamagedPrefab != null) { undamagedPrefab.SetActive(false); }
        if (damagedPrefab != null) { damagedPrefab.SetActive(false); }


        // Uses Overlap sphere to draw rays to all damageable objects within range
        // If it hits something with an HP value, it'll damage it (Enemies, Player, Breakables).
        // If it hits something with a rigidbody, it'll apply force.
        if (explosive && !EXPLODED)
        {
            if (explosionParticles != null) { explosionParticles.Play(); }

            if (!explosionSound.IsNull) { RuntimeManager.PlayOneShot(explosionSound, this.gameObject.transform.position); }
            
            EXPLODED = true;
            // DO NOT TOUCH THIS VARIABLE
            bool playerHurt = false;
            // Ensures the player only gets hit once per object explosion

            Vector3 explodePos = gameObject.transform.position;
            int hits = Physics.OverlapSphereNonAlloc(explodePos, explosionRadius, fragmentHits, damageLayer, QueryTriggerInteraction.Collide);

            for (int i = 0; i < hits; i++)
            {
                if (fragmentHits[i].gameObject == this.gameObject || fragmentHits[i].gameObject.transform.root == this.gameObject) continue;
                float distance = Vector3.Distance(explodePos, fragmentHits[i].transform.position);
                // Breaks Objects
                if (fragmentHits[i].TryGetComponent<BreakableObject>(out BreakableObject obj))
                {
                    //float distance = Vector3.Distance(explodePos, fragmentHits[i].transform.position);

                    if (!Physics.Raycast(explodePos, (fragmentHits[i].transform.position - explodePos).normalized, distance, blockFragmentsLayer.value) && !obj.EXPLODED)
                    {
                        Debug.DrawLine(explodePos, fragmentHits[i].transform.position, Color.green, 5f);
                        obj.DestructionPos = explodePos;
                        obj.Damage(Mathf.Lerp(maxDamage, minDamage, distance / explosionRadius));
                    }
                }
                // Hurts Enemies
                if (fragmentHits[i].TryGetComponent<IEnemy>(out IEnemy enemy))
                {
                    //float distance = Vector3.Distance(explodePos, fragmentHits[i].transform.position);

                    if (!Physics.Raycast(explodePos, (fragmentHits[i].transform.position - explodePos).normalized, distance, blockFragmentsLayer.value))
                    {
                        Debug.DrawLine(explodePos, fragmentHits[i].transform.position, Color.green, 5f);
                        enemy.Damage(Mathf.Lerp(maxDamage, minDamage, distance / explosionRadius));
                    }
                }
                else if (fragmentHits[i].TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    if (!Physics.Raycast(explodePos, (fragmentHits[i].transform.position - explodePos).normalized, distance, blockFragmentsLayer.value))
                    {
                        Debug.DrawLine(explodePos, fragmentHits[i].transform.position, Color.green, 5f);
                        damageable.Damage(Mathf.Lerp(maxDamage, minDamage, distance / explosionRadius));
                    }
                }
                // Hurts Player
                if (fragmentHits[i].TryGetComponent<PlayerBehavior>(out PlayerBehavior player))
                {
                    //float distance = Vector3.Distance(explodePos, fragmentHits[i].transform.position);

                    if (!Physics.Raycast(explodePos, (fragmentHits[i].transform.position - explodePos).normalized, distance, blockFragmentsLayer.value) && !playerHurt)
                    {
                        Debug.DrawLine(explodePos, fragmentHits[i].transform.position, Color.green, 5f);
                        player.Damage(Mathf.Lerp(maxDamage, minDamage, distance / explosionRadius));
                        playerHurt = true;
                    }
                }
                // Rigidbody Forces
                if (fragmentHits[i].TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    //float distance = Vector3.Distance(explodePos, fragmentHits[i].transform.position);

                    if (!Physics.Raycast(explodePos, (fragmentHits[i].transform.position - explodePos).normalized, distance, blockFragmentsLayer.value))
                    {
                        Debug.DrawLine(explodePos, fragmentHits[i].transform.position, Color.blue, 5f);
                        rb.AddExplosionForce(explosionForce, explodePos, explosionRadius);
                    }
                }
            }
        }

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
                if (renderer.bounds.size.sqrMagnitude != 0)
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
            body.AddExplosionForce(destructionForce, destructionPos, destructionRadius);
        }

        // Calls for the broken pieces to begin fading out
        StartCoroutine(FadeOutRigidBodies(rigidbodies));
    }

    private Renderer GetRendererFromRigidbody(Rigidbody Rigidbody) { return Rigidbody.GetComponent<Renderer>(); }

    private void OnDrawGizmos()
    {
        if (explosive) 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius); 
        }
    }
}
