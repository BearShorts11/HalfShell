using Assets.Scripts;
using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class IPickup : MonoBehaviour, IBind<PickupData>
{
    public int regainAmount;
    public bool infinite;
    public bool rotate;
    public float rotateSpeed = 50f;

    private PlayerBehavior player;
    private PlayerShooting gun;
    private PlayerUI ui;
    public PlayerBehavior Player { get { return player; } set { player = value; } }
    public PlayerShooting Gun { get { return gun; } set { gun = value; } }
    public PlayerUI UI { get { return ui; } set { ui = value; } }

    public enum PickupType
    { 
        Ammo = 0,
        Armor = 1,
        Health = 2,
        Shotgun = 3
    }
    public PickupType Type;
    [SerializeField] bool isBig;

    //data saving
    [SerializeField] public PickupData data;
    [SerializeField] private SerializableGuid _id = new SerializableGuid(Guid.NewGuid());
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerBehavior>();
        gun = FindFirstObjectByType<PlayerShooting>();
        ui = FindFirstObjectByType<PlayerUI>();

        //if (!data.Saved && !data.FirstBind) Destroy(this.gameObject);
    }

    public void Rotate() => transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

    public void Bind(PickupData data)
    {
        if (this.data.FirstBind)
        {
            this.data.position = this.transform.position;
            this.data.rotation = this.transform.rotation;
            this.data.Type = (int)this.Type;
            this.data.IsBig = this.isBig;

            this.data.FirstBind = false;
        }
        else
        { 
            this.transform.position = data.position;
            this.transform.rotation = data.rotation;
        }

    }

    protected void BaseUpdate()
    {
        this.data.position = this.transform.position;
        this.data.rotation = this.transform.rotation;
    }

    public void OnPickup()
    {
        //ObjectManager manager = FindFirstObjectByType<ObjectManager>();
        //if (manager is null) return;

        //manager.PickedUpObject(this.Id);

        Kerth k = FindFirstObjectByType<Kerth>();
        if (k is not null)
        {
            k.PickedUpObject(this.data);
            if (Type == PickupType.Ammo)
            {
                AmmoPickup ammopickup = GetComponent<AmmoPickup>();
                if (ammopickup != null)
                    k.gameObject.GetComponent<PlayerBehavior>().NotifyPlayer($"You got {ammopickup.ammoType} " + $"{Type}".ToLower() + "!");
            }
            else
                k.gameObject.GetComponent<PlayerBehavior>().NotifyPlayer($"You got {Type}!");
        }
    }

}
