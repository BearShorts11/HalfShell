using System.Collections.Generic;
using UnityEngine;

public enum AmmoType
{
    HalfShell,
    Slug,
    BuckShot,
}

public class AmmoPickup : MonoBehaviour
{
    public int ammoRegainAmount;
    public bool infinite;
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
        if (rotate == true) { Rotate(); }
    }

    public void Rotate()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (ammoType == AmmoType.HalfShell)     { player.AmmoHalfShell(ammoRegainAmount); }
            else if (ammoType == AmmoType.Slug)     { player.AmmoSlug(ammoRegainAmount); }
            else if (ammoType == AmmoType.BuckShot) { player.AmmoBuckShot(ammoRegainAmount); }

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
