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
    }

    [ContextMenu("Load From Checkpoint")]
    public void LoadFromCheckpoint()
    {
        ObjectSaveData[] datas = FindObjectsByType<ObjectSaveData>(FindObjectsSortMode.None);

        foreach (ObjectSaveData data in datas)
        {
            data.OnLoad();
        }
    }
}