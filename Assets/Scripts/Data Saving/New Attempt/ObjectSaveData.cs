using UnityEngine;

public abstract class ObjectSaveData : MonoBehaviour, ISavableData
{
    public Vector3 Positon { get; set; }
    public Quaternion Rotation { get; set; }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void OnSave()
    {
        this.Positon = transform.position;
        this.Rotation = transform.rotation;
    }

    public virtual void OnLoad()
    { 
        transform.position = this.Positon;
        transform.rotation = this.Rotation;
    }
}
