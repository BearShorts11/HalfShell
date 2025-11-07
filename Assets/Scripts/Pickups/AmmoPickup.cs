using System.Collections.Generic;
using UnityEngine;


public class AmmoPickup : IPickup
{
    [SerializeField] private ShellBase.ShellType ammoType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Gun = FindFirstObjectByType<PlayerShooting>();
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
            bool canPickup = false;
            if (ammoType == ShellBase.ShellType.HalfShell)     { canPickup = Gun.AddAmmo(regainAmount, new HalfShell()); }
            else if (ammoType == ShellBase.ShellType.Slug)     { canPickup = Gun.AddAmmo(regainAmount, new Slug()); }
            else if (ammoType == ShellBase.ShellType.Buckshot) { canPickup = Gun.AddAmmo(regainAmount, new Buckshot()); }

            if (canPickup)
            {
                List<GameObject> buttons = new List<GameObject>();
                buttons.AddRange(GameObject.FindGameObjectsWithTag("ShellButton"));
                foreach (GameObject button in buttons)
                {
                    ShellSelectionButton counter = button.GetComponent<ShellSelectionButton>();
                    counter.UpdateAmmoCount();
                }

                // More to be added here when Ammo Maximums are added -A
                if (!infinite) { Destroy(gameObject); }
            }
        }
    }
}
