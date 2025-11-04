using UnityEngine;

public class ArmorPickup : IPickup
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
            if (Player.Armor >= Player.MaxArmor) { return; }

            Player.Armor += regainAmount;
            UI.UpdateArmor(Player.Armor, Player.MaxArmor);

            Destroy(gameObject);
        }
    }
}
