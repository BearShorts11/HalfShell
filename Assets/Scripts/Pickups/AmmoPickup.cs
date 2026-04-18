using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.Rendering;


public class AmmoPickup : IPickup
{
    [field: SerializeField] public ShellBase.ShellType ammoType { get; private set; } = ShellBase.ShellType.Slug;
    [SerializeField] private EventReference pickupSound;
    [SerializeField] private Animator animator;

    [SerializeField] private Renderer Rend;
    MaterialPropertyBlock matCollor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Gun = FindFirstObjectByType<PlayerShooting>();
        UI = FindFirstObjectByType<PlayerUI>();
        this.Type = PickupType.Ammo;
        SetSmallAmmoColor();
    }

    // Update is called once per frame
    void Update()
    {
        if (rotate) { Rotate(); }
        base.BaseUpdate();
    }

    private void OnValidate()
    {
        SetSmallAmmoColor();
    }

    private void SetSmallAmmoColor()
    {
        if (Rend != null) // Note: Rend is not automatically set. If the ammo pickup model has an extra material slot with a modular texture, set it in the editor prefab.
        {
            if (Rend.sharedMaterials.Length > 1) // Does this mesh use more than 1 material?
            {
                matCollor = new();
                switch (ammoType)
                {
                    case ShellBase.ShellType.Slug:
                        matCollor.SetColor("_Color", Color.green);
                        break;
                    case ShellBase.ShellType.Incindiary:
                        matCollor.SetColor("_Color", ColorsExt.orange);
                        break;
                    case ShellBase.ShellType.Buckshot:
                    case ShellBase.ShellType.HalfShell:
                        matCollor.SetColor("_Color", Color.red);
                        break;
                    case ShellBase.ShellType.BeanBag:
                    case ShellBase.ShellType.BMG:
                    default:
                        break;
                }
                if (!matCollor.isEmpty)
                {
                    Rend.SetPropertyBlock(matCollor, 1); // The modular material should always be in index 1.
                }
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            bool canPickup = false;
            if (ammoType == ShellBase.ShellType.HalfShell)     { canPickup = Gun.AddAmmo(regainAmount, new HalfShell()); }
            else if (ammoType == ShellBase.ShellType.Slug)     { canPickup = Gun.AddAmmo(regainAmount, new Slug()); }
            else if (ammoType == ShellBase.ShellType.Buckshot) { canPickup = Gun.AddAmmo(regainAmount, new Buckshot()); }
            else if (ammoType == ShellBase.ShellType.Incindiary) { canPickup = Gun.AddAmmo(regainAmount, new Incindiary()); }

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
                else
                    PickupMessage(other.gameObject.GetComponent<PlayerBehavior>(), $"You got {ammoType} " + $"{Type}".ToLower() + "!");
            }
        }
    }

    public override void PickupMessage(PlayerBehavior Player, string Message)
    {
        Player.NotifyPlayer(Message);
    }
}
