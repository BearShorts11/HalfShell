using UnityEngine;

public enum AmmoType
{
    HalfShell,
    Slug,
    BuckShot,
}

public class AmmoPickup : MonoBehaviour
{
    public GameObject ammoPickup;
    public int ammoRegainAmount;
    public bool rotate;
    public float rotateSpeed = 50f;

    public AmmoType ammoType;

    private PlayerShooting player;
    private PlayerUI playerUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerShooting>();
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
            if (ammoType == AmmoType.HalfShell)
            {
                other.GetComponent<PlayerShooting>().AmmoHalfShell(ammoRegainAmount);
                Destroy(ammoPickup);
            }
            else if (ammoType == AmmoType.Slug)
            {
                other.GetComponent<PlayerShooting>().AmmoSlug(ammoRegainAmount);
                Destroy(ammoPickup);
            }
            else if (ammoType == AmmoType.BuckShot)
            {
                other.GetComponent<PlayerShooting>().AmmoBuckShot(ammoRegainAmount);
                Destroy(ammoPickup);
            }
        }
    }
}
