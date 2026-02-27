using Assets.Scripts;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class Kerth : MonoBehaviour, IBind<PlayerData>
{
    [SerializeField] private SerializableGuid _id = new SerializableGuid(Guid.NewGuid());
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [SerializeField] PlayerData data;

    [SerializeField] PlayerBehavior behavior;
    [SerializeField] PlayerShooting shooting;

    private void Start()
    {
        //number based on shell type enum, can't think of a better way to do this right now
        data.AmmoCounts = new int[6];
    }

    public void Bind(PlayerData data)
    {
        this.data = data;
        this.data.Id = Id;
        transform.position = data.position;
        transform.rotation = data.rotation;

        behavior.SetHealth(data.Health);
        behavior.SetArmor(data.Armor);

        shooting.SetAmmoCounts(data.AmmoCounts);
        shooting.SetMagazine(data.ReversedMagazine);
        shooting.SetChamber(ConvertNumToShell());
    }

    private void Update()
    {
        //behavior data
        data.position = transform.position;
        data.rotation = transform.rotation;
        data.Health = behavior.Health;
        data.Armor = behavior.Armor;

        //shooting data
        data.AmmoCounts[(int)ShellBase.ShellType.HalfShell] = shooting.AmmoCounts[ShellBase.ShellType.HalfShell];
        data.AmmoCounts[(int)ShellBase.ShellType.Slug] = shooting.AmmoCounts[ShellBase.ShellType.Slug];
        if (shooting.Chamber is not null) data.Chamber = (int)shooting.Chamber.Type;
        else data.Chamber = default;
    }

    private ShellBase ConvertNumToShell()
    {
        switch (data.Chamber)
        {
            case 1:
                return new HalfShell();
            case 2:
                return new Slug();
                break;
            default:
                return null;
                break;
        }
    }

    /// <summary>
    /// heafty operations that should not be saved every frame and only when the game is saved
    /// </summary>
    public void OnSave()
    {
        data.ReversedMagazine = new int[shooting.Magazine.Count];
        int index = 0;

        //stores shells from shooting.mag before putting them back in shooting.mag
        Stack<ShellBase> reserve = new Stack<ShellBase>(); 

        while (shooting.Magazine.Count > 0)
        { 
            ShellBase shell = shooting.Magazine.Pop();
            data.ReversedMagazine[index] = ((int)(shell.Type));
            index++;
            reserve.Push(shell);
        }

        //above operation takes shells out of mag, this puts them back in so player can keep playing
        while (reserve.Count > 0)
        {
            shooting.Magazine.Push(reserve.Pop());
        }
    }

}