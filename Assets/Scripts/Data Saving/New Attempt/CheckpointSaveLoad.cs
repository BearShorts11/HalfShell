using UnityEngine;

public class CheckpointSaveLoad : MonoBehaviour
{
    private void Awake()
    {
        Checkpoint.SaveGame.AddListener(SaveAtCheckpoint);
    }

    [ContextMenu("Save At Checkpoint")]
    public void SaveAtCheckpoint()
    {
        Debug.Log("saving from checkpoint");

        ObjectSaveData[] datas = FindObjectsByType<ObjectSaveData>(FindObjectsSortMode.None);

        foreach (ObjectSaveData data in datas)
        { 
            data.OnSave();
        }

        //if player manages to reach another checkpoint without picking up an enemy object, it can stay as it was saved
        IPickup[] pickups = FindObjectsByType<IPickup>(FindObjectsSortMode.None);
        foreach (IPickup pickup in pickups)
        {
            if (pickup.droppedFromEnemy) pickup.droppedFromEnemy = false;
        }
    }

    [ContextMenu("Load From Checkpoint")]
    public void LoadFromCheckpoint()
    {
        ObjectSaveData[] datas = FindObjectsByType<ObjectSaveData>(FindObjectsSortMode.None);

        foreach (ObjectSaveData data in datas)
        {
            data.OnLoad();
        }

        //destory pickups dropped from enemies as the enemies would be reset on reloading
        IPickup[] pickups = FindObjectsByType<IPickup>(FindObjectsSortMode.None);
        foreach (IPickup pickup in pickups)
        { 
            if (pickup.droppedFromEnemy) Destroy(pickup.gameObject);
        }
    }
}