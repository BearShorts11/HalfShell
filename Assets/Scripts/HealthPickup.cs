using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public GameObject healthPickup;
    public float healthRegainAmount;
    public float rotateSpeed = 50f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    public void Rotate()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerBehavior>().Health += healthRegainAmount;
            Debug.Log(other.gameObject.GetComponent<PlayerBehavior>().Health);
            Destroy(healthPickup);
        }
    }
}
