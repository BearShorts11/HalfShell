using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Data;
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
        this.data.EnemiesInScene = data.EnemiesInScene;
        if(data.FirstSaveHappened) OnReload();
    }

    private void Update()
    {

    }

    public void OnSave()
    {
        Fiend[] enemiesInScene = FindObjectsByType<Fiend>(FindObjectsSortMode.None);

        data.EnemiesInScene = new SerializableGuid[enemiesInScene.Length];

        for (int i = 0; i < enemiesInScene.Length; i++)
        {
            data.EnemiesInScene[i] = enemiesInScene[i].Id;
        }

        data.FirstSaveHappened = true;
    }

    public void OnReload()
    {
        Fiend[] enemiesInScene = FindObjectsByType<Fiend>(FindObjectsSortMode.None);

        data.EnemiesInScene = new SerializableGuid[enemiesInScene.Length];

        for (int i = 0; i < enemiesInScene.Length; i++)
        {
            bool inScene = false;

            for (int j = 0; j < data.EnemiesInScene.Length; j++)
            {
                //prevents error after destroying
                if (enemiesInScene[i] is null) break;

                if (enemiesInScene[i].Id == data.EnemiesInScene[j]) inScene = true;
            }

            if (!inScene) Destroy(enemiesInScene[i].gameObject);
        }

    }

}
