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

    public Vector3 position;
    public Quaternion rotation;
    public bool FirstBind = true;
    public int Type;
    public bool IsBig;
    //add amt?
}
