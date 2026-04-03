using UnityEngine;

public abstract class ObjectSaveData : MonoBehaviour, ISavableData
{
    public Vector3 Positon { get; set; }
    public Quaternion Rotation { get; set; }

    //specifically properties so as to no expose to editor
    public Vector3 lastPosition { get; protected set; }
    public Quaternion lastRotation { get; protected set; }

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

    /// <summary>
    /// sets the last position of the transform when picked up. Last positions are used when the level is re
    /// loaded to respawn pickups that were picked up since the last save by the player
    /// </summary>
    public void SetLastTransform()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}
