using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using Unity.Cinemachine;
using Unity.VisualScripting;
using PixelCrushers.DialogueSystem;

public class PlayerLuaRef : PlayerShooting
{
    //public static bool canFire = true;
    public string FirstShot = "no shot";


    void Update()
    {


        if (canFire && Input.GetButtonDown("Fire1")) 
        {
          FirstShot = "yes shot";
            Debug.Log("FirstShot true");
        }
        

    }

    public bool ShootFirstShot(string ShootFirst)
    {
        return FirstShot == ShootFirst;
    }

    private void OnEnable()
    {
        Lua.RegisterFunction("ShootFirstShot", this, SymbolExtensions.GetMethodInfo(() => ShootFirstShot(string.Empty)));
    }

    private void OnDisable()
    {
        Lua.UnregisterFunction("ShootFirstShot");
    }
}