using System.Collections.Generic;
using UnityEngine;

public enum AmmoType
{
    HalfShell,
    Slug,
    BuckShot,
}

public class AmmoPickup : IPickup
{
    [SerializeField] private AmmoType ammoType;

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
            if (ammoType == AmmoType.HalfShell)     { Gun.AmmoHalfShell(regainAmount); }
            else if (ammoType == AmmoType.Slug)     { Gun.AmmoSlug(regainAmount); }
            else if (ammoType == AmmoType.BuckShot) { Gun.AmmoBuckShot(regainAmount); }

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
