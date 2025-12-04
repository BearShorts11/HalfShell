using Assets.Scripts;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Kerth : MonoBehaviour, IBind<PlayerData>
{
    [SerializeField] private SerializableGuid _id = new SerializableGuid(Guid.NewGuid());
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [SerializeField] PlayerData data;

    //[SerializeField] PlayerBehavior behavior;
    //[SerializeField] PlayerShooting shooting;

    public void Bind(PlayerData data)
    {
        this.data = data;
        this.data.Id = Id;
        transform.position = data.position; // WHY NO WORKY!?!?!?!
        transform.rotation = data.rotation;

        //behavior.SetHealth(data.Health);
        //behavior.SetArmor(data.Armor);
    }

    private void Update()
    {
        data.position = transform.position;
        data.rotation = transform.rotation;

        //data.Health = behavior.Health;
        //data.Armor = behavior.Armor;
    }

}