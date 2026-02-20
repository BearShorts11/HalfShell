using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 150F;
    private Vector3 target;
    private float targetReached = 0.001f;

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
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target) <= targetReached) Destroy(this.gameObject);

        }
        else transform.Translate(Vector3.forward * speed * Time.deltaTime);
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
