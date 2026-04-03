using UnityEngine;

public abstract class ObjectSaveData : MonoBehaviour, ISavableData
{
    public Vector3 Positon { get; set; }
    public Quaternion Rotation { get; set; }

    protected bool hasBeenSaved;

    public virtual void OnSave()
    {
        hasBeenSaved = true;    
        this.Positon = transform.position;
        this.Rotation = transform.rotation;
    }

    public virtual void OnLoad()
    {
        if (!hasBeenSaved) return;
        transform.position = this.Positon;
        transform.rotation = this.Rotation;
    }
}
