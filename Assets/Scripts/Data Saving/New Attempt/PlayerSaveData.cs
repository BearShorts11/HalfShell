using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveData : ObjectSaveData
{

    public float Health { get; private set; }
    public float Armor { get; private set; }

    private PlayerBehavior behavior;
    private PlayerShooting shooting;

    public Dictionary<ShellBase.ShellType, int> AmmoCounts = new Dictionary<ShellBase.ShellType, int>()
    {
        { ShellBase.ShellType.Buckshot, 0 },
        { ShellBase.ShellType.HalfShell, 0 },
        { ShellBase.ShellType.Slug, 0 }
    };

    /// <summary>
    /// because *stacks* magazine is saved in reversed order and loaded in correct order. 
    /// Saved as ints to reference shell type in ShellBase.ShellType enum. 
    /// Cannot save custom objects directly
    /// </summary>
    public Stack<ShellBase> ReversedMagazine = new Stack<ShellBase>();

    /// <summary>
    /// holds enum number of shell type
    /// </summary>
    public ShellBase.ShellType Chamber;

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
        //i'm doing a bad i know
        shooting.AmmoCounts[ShellBase.ShellType.HalfShell] = this.AmmoCounts[ShellBase.ShellType.HalfShell];
        shooting.AmmoCounts[ShellBase.ShellType.Slug] = this.AmmoCounts[ShellBase.ShellType.Slug];

    }

    private void LoadMagazine()
    { 
        shooting.Magazine.Clear();

        while (this.ReversedMagazine.Count > 0)
        {
            shooting.Magazine.Push(ReversedMagazine.Pop());
        }
    }
}
