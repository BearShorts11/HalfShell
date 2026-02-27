using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Holds which enemies and pickups are currently in the scene
/// </summary>
public class SceneData : ISaveable
{
    [SerializeField] private SerializableGuid _id;
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    /// <summary>
    /// Used to determine if the scene has been loaded once before or not. 
    /// Used to determine which enemies should be in the scene or not on reloading.
    /// </summary>
    public bool FirstSaveHappened;
    public SerializableGuid[] EnemiesInScene;
    //public IPickup[] PickupsInScene;
}
