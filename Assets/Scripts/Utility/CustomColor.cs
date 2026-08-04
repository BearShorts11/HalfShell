using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;

/// <summary>
/// Custom Colors
/// Recolor a game object's material individually without changing the colors of others using the same material
/// (provided the material shader has a property named "_BaseColor" by default anyway)
/// </summary>
public class CustomColor : MonoBehaviour
{
    public string propertyName = "_Color";
    public Color color = Color.white;
    MaterialPropertyBlock matProp;
    List<Renderer> Rends = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateColor(color);
    }

    private void OnValidate()
    {
        UpdateColor(color);
    }

    public void UpdateColor(Color color)
    {
        ClearColor();
        Rends = this.gameObject.GetComponentsInChildren<Renderer>().ToList<Renderer>();
        matProp ??= new();

        if (Rends.Count != 0)
        {
            for (int i = 0; i < Rends.Count; i++)
            {
                if (!Rends[i].HasPropertyBlock())
                {
                    for (int k = 0; k < Rends[i].sharedMaterials.Length; k++)
                    {
                        matProp.SetColor(propertyName, Rends[i].sharedMaterials[k].GetColor(propertyName) * color);
                        Rends[i].SetPropertyBlock(matProp, k);
                    }
                }
            }
        }
    }

    public void ClearColor()
    {
        if (Rends == null ) return;
        if (Rends.Count != 0)
        {
            for (int i = 0; i < Rends.Count; i++)
            {
                if (Rends[i] == null) continue;

                if (Rends[i].HasPropertyBlock())
                {
                    for (int k = 0; k < Rends[i].sharedMaterials.Length; k++)
                    {
                        Rends[i].SetPropertyBlock(null, k);
                    }
                }
            }
        }
        Rends.Clear();
        Rends.TrimExcess();
    }
}
