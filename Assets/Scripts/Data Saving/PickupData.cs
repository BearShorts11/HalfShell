using Assets.Scripts;
using System;
using UnityEngine;

[Serializable]
public class PickupData : ISaveable
{
    [SerializeField] private SerializableGuid _id;
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public bool Saved = false;
    public bool FirstBind = true;
}
