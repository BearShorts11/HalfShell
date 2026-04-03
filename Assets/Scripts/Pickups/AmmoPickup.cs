using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class AmmoPickup : IPickup
{
    [SerializeField] private ShellBase.ShellType ammoType;
    [SerializeField] private EventReference pickupSound;
    [SerializeField] private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Gun = FindFirstObjectByType<PlayerShooting>();
        UI = FindFirstObjectByType<PlayerUI>();
        this.Type = PickupType.Ammo;
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
                RuntimeManager.PlayOneShot(pickupSound, transform.position);
                List<GameObject> buttons = new List<GameObject>();
                buttons.AddRange(GameObject.FindGameObjectsWithTag("ShellButton"));
                foreach (GameObject button in buttons)
                {
                    ShellSelectionButton counter = button.GetComponent<ShellSelectionButton>();
                    counter.UpdateAmmoCount();
                   
                }

                // If this is an ammo crate that *has* an animation for picking up ammo, play this animation state -V
                if (animator != null)
                    animator.CrossFade("Ammo_Pickup", 0.2f);

                // More to be added here when Ammo Maximums are added -A
                if (!infinite) { base.OnPickup(); Destroy(gameObject); }
            }
        }
    }
}
