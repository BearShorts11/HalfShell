using UnityEngine;
using UnityEngine.UIElements;

public class HealthPickup : MonoBehaviour
{
    public GameObject healthPickup;
    public float healthRegainAmount;
    public bool rotate;
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
        if (rotate == true)
        {
            Rotate();
        }
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
            playerUI.UpdateHP(player.Health, player.MaxHP);
            playerUI.CheckHealth();

            Debug.Log(other.gameObject.GetComponent<PlayerBehavior>().Health);

            Destroy(healthPickup);
        }
    }
}
