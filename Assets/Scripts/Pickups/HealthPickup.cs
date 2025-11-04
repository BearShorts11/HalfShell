using UnityEngine;
using UnityEngine.UIElements;

public class HealthPickup : IPickup
{
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
            if (Player.Health >= Player.MaxHP) { return; }

            Player.Health += regainAmount;
            UI.UpdateHP(Player.Health, Player.MaxHP);
            UI.CheckHealth();

            Destroy(gameObject);
        }
    }
}
