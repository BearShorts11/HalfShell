using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 150F;
    private Vector3 target;
    private float targetReached = 0.001f;
    Vector3 nextPos;
    Collider objectToBeHit;

    /// <summary>
    /// Prevents a double hit issue where it would damage the player twice
    /// </summary>
    private bool hitSomething;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, 5f);
        if (target != null) transform.LookAt(target);
    }

    private void FixedUpdate()
    {
        // from the prediction by the end of this method: if there was something about to be hit, call this method
        if (objectToBeHit != null) OnTriggerEnter(objectToBeHit);

        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target) <= targetReached) Destroy(this.gameObject);
        }
        else transform.Translate(Vector3.forward * speed * Time.deltaTime);
        nextPos = transform.position + (transform.forward * speed * Time.deltaTime);

        //Predict if the bullet will hit the player or something by the next update
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, speed * Time.deltaTime, 1<<0|1<<4|1<<6|1<<7|1<<9|1<<12))
        {
            if (hit.collider != null)
                objectToBeHit = hit.collider;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (transform.parent is not null && !hitSomething)
            {
                hitSomething = true; //prevents double hit issue
                other.GetComponent<IDamageable>().TakeDamage(GetComponentInParent<Enemy>().damage);
            }
        }

        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }

    public void GiveTarget(Vector3 target)
    { 
        this.target = target;
    }
}
