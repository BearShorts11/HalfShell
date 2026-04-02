using UnityEngine;
using System;

public interface ISavableData
{
    public Vector3 Positon { get; set; }
    public Quaternion Rotation { get; set; }

    public void OnSave();
    public void OnLoad();
}
