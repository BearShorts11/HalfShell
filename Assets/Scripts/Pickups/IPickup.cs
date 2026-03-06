using Assets.Scripts;
using System;
using UnityEngine;

public class IPickup : MonoBehaviour, IBind<PickupData>
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

        if (!data.Saved && !data.FirstBind) Destroy(this.gameObject);
    }

    public void Rotate() => transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

    public void Bind(PickupData data)
    {
        this.data = data;
        this.data.Id = Id;
        this.data.Saved = data.Saved;
    }

    public void OnPickup()
    {
        ObjectManager manager = FindFirstObjectByType<ObjectManager>();
        if (manager is null) return;

        manager.PickedUpObject(this.Id);
    }

}
