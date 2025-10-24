using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public GameObject ammoPickup;
    public float ammoRegainAmount;
    public float rotateSpeed = 50f;

    private PlayerBehavior player;
    private PlayerUI playerUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerBehavior>();
        playerUI = FindFirstObjectByType<PlayerUI>();
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
            Destroy(ammoPickup);
        }
    }
}
