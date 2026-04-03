using Assets.Scripts;
using System;
using UnityEngine;

public abstract class IPickup : MonoBehaviour
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
    public PickupType Type; //set by subclasses
    public bool isBig; //set in editor

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerBehavior>();
        gun = FindFirstObjectByType<PlayerShooting>();
        ui = FindFirstObjectByType<PlayerUI>();
    }

    public void Rotate() => transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

    public void OnPickup()
    {
        PlayerSaveData playerData = FindFirstObjectByType<PlayerSaveData>();
        if (playerData is not null)
        {
            //pick up object
            PickupSaveData saveData = this.GetComponent<PickupSaveData>();
            saveData.SetLastTransform();
            playerData.PickedUpObject(saveData);
        }
    }

}
