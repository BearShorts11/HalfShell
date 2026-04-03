using System;
using System.Collections.Generic;
using UnityEngine;

public class PickupSaveData : ObjectSaveData
{
    //don't want these exposed to the editor, want these values set from IPickup
    public IPickup.PickupType Type { get; private set; }
    public bool IsBig { get; private set; }

    private void Start()
    {
        this.Type = GetComponent<IPickup>().Type;
        this.IsBig = GetComponent<IPickup>().isBig;
    }
}