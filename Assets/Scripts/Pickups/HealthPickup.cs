using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;
using FMOD.Studio;

public class HealthPickup : IPickup
{
    [SerializeField] private EventReference pickupSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = FindFirstObjectByType<PlayerBehavior>();
        UI = FindFirstObjectByType<PlayerUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate) { Rotate(); }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (Player.Health >= Player.maxHealth) { return; }
            RuntimeManager.PlayOneShot(pickupSound, transform.position);
            Player.Health += regainAmount;
            UI.UpdateHP(Player.Health, Player.maxHealth);
            UI.CheckHealth();

            Destroy(gameObject);
        }
    }
}
