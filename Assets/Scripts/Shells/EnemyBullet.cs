using Assets.Scripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 200f;
    private Vector3 target;
    private float targetReached = 0.001f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, 5f);
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
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerBehavior>().Damage(RangedEnemy.gunDamage);
        }

        Debug.Log(other.gameObject.name);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy (this.gameObject);
    }

    public void GiveTarget(Vector3 target)
    { 
        this.target = target;
    }
}
