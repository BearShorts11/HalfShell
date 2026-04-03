using System;
using System.Collections.Generic;
using UnityEngine;

public class PickupSaveData : ObjectSaveData
{
    //don't want these exposed to the editor, want these values set from IPickup
    public IPickup.PickupType Type { get; private set; }
    public bool IsBig { get; private set; }

    public Vector3 lastPosition;
    public Quaternion lastRotation;

    private void Start()
    {
        this.Type = GetComponent<IPickup>().Type;
        this.IsBig = GetComponent<IPickup>().isBig;
    }

    public void SetLastTransform()
    {
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
}