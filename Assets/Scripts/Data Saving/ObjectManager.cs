using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ObjectManager : MonoBehaviour, IBind<SceneData>
{
    [SerializeField] private SerializableGuid _id = new SerializableGuid(Guid.NewGuid());
    public SerializableGuid Id
    {
        get { return _id; }
        set { _id = value; }
    }

    [SerializeField] SceneData data;

    public void Bind(SceneData data)
    {
        this.data = data;
        this.data.Id = Id;
    }

    public void OnSave()
    {
        
    }

    public void OnReload()
    {
        IPickup[] pickupsInScene = FindObjectsByType<IPickup>(FindObjectsSortMode.None);
        for (int i = 0; i < pickupsInScene.Length; i++)
        {
            foreach (SerializableGuid id in data.PickedUpObjects)
            {
                if (pickupsInScene[i].Id == id) Destroy(pickupsInScene[i].gameObject);
            }
        }

        Fiend[] enemiesInScene = FindObjectsByType<Fiend>(FindObjectsSortMode.None);
        for (int i = 0; i < enemiesInScene.Length; i++)
        {
            foreach (SerializableGuid id in data.DeadEnemies)
            {
                if (enemiesInScene[i].Id == id) Destroy(enemiesInScene[i].gameObject);
            }
        }
    }

    public void PickedUpObject(SerializableGuid id)
    { 
        data.PickedUpObjects.Add(id);  
    }

    public void DeadEnemy(SerializableGuid id)
    { 
        data.DeadEnemies.Add(id);
    }
}
