using Assets.Scripts;
using System;
using UnityEngine;

[Serializable]
public class EnemyData : ISaveable
{
    [SerializeField] private SerializableGuid _id;
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    //basic data- no need to save exact rotation
    public Vector3 position;
    public float Health;

    /// <summary>
    /// converted to and from state object in Fiend (object will not save)
    /// </summary>
    public string State;
}
