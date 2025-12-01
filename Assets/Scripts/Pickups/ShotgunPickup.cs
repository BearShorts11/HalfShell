using FMODUnity;
using UnityEngine;

public class ShotgunPickup : IPickup
{
    [SerializeField] private EventReference pickupSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = FindFirstObjectByType<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate == true)
        {
            Rotate();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RuntimeManager.PlayOneShot(pickupSound, transform.position);
            Player.EquipShotgun();

            Destroy(gameObject);
        }
    }
}
