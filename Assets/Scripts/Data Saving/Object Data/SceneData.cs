using Assets.Scripts;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds which enemies and pickups are currently in the scene
/// </summary>
[Serializable]
public class SceneData : ISaveable
{
    [SerializeField] private SerializableGuid _id;
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    //public SerializableGuid[] EnemiesInScene;
    public List<SerializableGuid> PickedUpObjects;
    public List<SerializableGuid> DeadEnemies;
}
