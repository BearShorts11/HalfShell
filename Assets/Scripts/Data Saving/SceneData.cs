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

    public Fiend[] EnemiesInScene;
    //public IPickup[] PickupsInScene;
}
