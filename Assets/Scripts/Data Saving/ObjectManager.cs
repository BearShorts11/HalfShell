using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        Fiend[] enemiesOnReaload = FindObjectsByType<Fiend>(FindObjectsSortMode.None);
        //IPickup[] pickupsOnReload = FindObjectsByType<IPickup>(FindObjectsSortMode.None);

        //if the fiend currently in the scene was not in the scene at the time of the last save,
        //that means the enemy was already killed, and should not be in the scene
        foreach (Fiend f in enemiesOnReaload)
        {
            bool wasInScene = false;
            foreach (Fiend fiend in data.EnemiesInScene)
            { 
                if (fiend.Id == f.Id) wasInScene = true;
            }
            if (!wasInScene) Destroy(f.gameObject);
        }

        //same for pickups
        
    }

    private void Update()
    {
        data.EnemiesInScene = FindObjectsByType<Fiend>(FindObjectsSortMode.None);
        //data.PickupsInScene = FindObjectsByType<IPickup>(FindObjectsSortMode.None);
    }

}
