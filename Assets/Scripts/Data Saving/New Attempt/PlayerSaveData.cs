using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveData : ObjectSaveData
{
    [Header("Basic Data")]
    public float Health { get; private set; }
    public float Armor { get; private set; }

    private PlayerBehavior behavior;
    private PlayerShooting shooting;

    [Header("Shooting Data")]
    public Dictionary<ShellBase.ShellType, int> AmmoCounts = new Dictionary<ShellBase.ShellType, int>()
    {
        { ShellBase.ShellType.Buckshot, 0 },
        { ShellBase.ShellType.HalfShell, 0 },
        { ShellBase.ShellType.Slug, 0 }
    };

    /// <summary>
    /// saves the magazine in reverse because of how stacks work. 
    /// </summary>
    public Stack<ShellBase> ReversedMagazine = new Stack<ShellBase>();
    public ShellBase.ShellType Chamber { get; private set; }

    [Header("Pickup Data")]
    private List<PickupSaveData> pickupsSinceLastSave = new List<PickupSaveData>();

    [SerializeField] GameObject BigHealthPrefab;
    [SerializeField] GameObject SmallHealthPrefab;
    [SerializeField] GameObject BigAmmoPrefab;
    [SerializeField] GameObject SmallAmmoPrefab;
    [SerializeField] GameObject BigArmorPrefab;
    [SerializeField] GameObject SmallArmorPrefab;

    [Header("Enemy Data")]
    private List<EnemySaveData> gibbedEnemiesSinceLastSave = new List<EnemySaveData>();

    [SerializeField] GameObject MeleeBasicEnemyPrefab;
    [SerializeField] GameObject RangedBasicEnemyPrefab;

    private void Start()
    {
        behavior = GetComponent<PlayerBehavior>();
        shooting = GetComponent<PlayerShooting>();
    }

    public override void OnSave()
    {
        base.OnSave();

        this.Health = behavior.Health;
        this.Armor = behavior.Armor;

        this.AmmoCounts[ShellBase.ShellType.HalfShell] = shooting.AmmoCounts[ShellBase.ShellType.HalfShell];
        this.AmmoCounts[ShellBase.ShellType.Slug] = shooting.AmmoCounts[ShellBase.ShellType.Slug];

        if (shooting.Chamber is not null) this.Chamber = shooting.Chamber.Type;

        SaveMagazine();

        pickupsSinceLastSave.Clear();
        gibbedEnemiesSinceLastSave.Clear();
    }

    private void SaveMagazine()
    { 
        Stack<ShellBase> reserve = new Stack<ShellBase>();

        while (shooting.Magazine.Count > 0)
        { 
            ShellBase shell = shooting.Magazine.Pop();

            switch (shell.Type)
            { 
                case ShellBase.ShellType.HalfShell:
                    this.ReversedMagazine.Push(new HalfShell());
                    break;
                case ShellBase.ShellType.Slug:
                    this.ReversedMagazine.Push(new Slug());
                    break;
            }

            reserve.Push(shell);
        }

        while (reserve.Count > 0)
        {
            shooting.Magazine.Push(reserve.Pop());
        }
    }

    public override void OnLoad()
    {
        if (!hasBeenSaved) return; //only loads if there's actually data to load in

        //must disable the controller in order to move the player
        CharacterController controller = this.GetComponent<CharacterController>();
        controller.enabled = false;
        base.OnLoad();
        controller.enabled = true;

        behavior.SetHealth(Health);
        behavior.SetArmor(Armor);

        //load in saved chamber & mag
        switch (Chamber)
        {
            case ShellBase.ShellType.HalfShell:
                shooting.SetChamber(new HalfShell());
                break;
            case ShellBase.ShellType.Slug:
                shooting.SetChamber(new Slug());
                break;
        }

        shooting.SetMagazine(ReversedMagazine);

        //set ammo counts
        //i'm doing a bad i know I should be calling a method not directly changing shit
        shooting.AmmoCounts[ShellBase.ShellType.HalfShell] = this.AmmoCounts[ShellBase.ShellType.HalfShell];
        shooting.AmmoCounts[ShellBase.ShellType.Slug] = this.AmmoCounts[ShellBase.ShellType.Slug];

        ResetPickups();
        ReviveGibbedEnemies();
    }

    private void ResetPickups()
    {
        foreach (PickupSaveData data in pickupsSinceLastSave)
        {
            switch (data.Type)
            {
                case IPickup.PickupType.Ammo:
                    if (data.IsBig) Instantiate(BigAmmoPrefab, data.lastPosition, data.lastRotation);
                    else Instantiate(SmallAmmoPrefab, data.lastPosition, data.lastRotation);
                    break;
                case IPickup.PickupType.Armor:
                    if (data.IsBig) Instantiate(BigArmorPrefab, data.lastPosition, data.lastRotation);
                    else Instantiate(SmallArmorPrefab, data.lastPosition, data.lastRotation);
                    break;
                case IPickup.PickupType.Health:
                    if (data.IsBig) Instantiate(BigHealthPrefab, data.lastPosition, data.lastRotation);
                    else Instantiate(SmallHealthPrefab, data.lastPosition, data.lastRotation);
                    break;
            }
        }

        pickupsSinceLastSave.Clear();
    }

    private void ReviveGibbedEnemies()
    {
        foreach (EnemySaveData e in gibbedEnemiesSinceLastSave)
        {
            //e.gameObject.SetActive(true);
            //e.Enemy.Revive();
            Enemy enemy = e.Enemy;
            EnemySaveData newEnemy;

            switch (enemy)
            {
                case RangedEnemy:
                    newEnemy = Instantiate(RangedBasicEnemyPrefab, e.lastPosition, e.lastRotation).GetComponent<EnemySaveData>();
                    break;
                default:
                    newEnemy = Instantiate(MeleeBasicEnemyPrefab, e.lastPosition, e.lastRotation).GetComponent<EnemySaveData>();
                    break;
            }

            EnemySaveData data = newEnemy.GetComponent<EnemySaveData>();
            data.OnLoad();

            Destroy(e.gameObject);
        }

        gibbedEnemiesSinceLastSave.Clear();
    }

    public void PickedUpObject(PickupSaveData pickup)
    { 
        pickupsSinceLastSave.Add(pickup);
    }

    public void GibbedEnemy(EnemySaveData enemy)
    { 
        gibbedEnemiesSinceLastSave.Add(enemy);
    }

}
