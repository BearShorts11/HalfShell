using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;

/// <summary>
/// Custom Texture
/// Retexture a game object's material individually without changing the textures of others using the same material
/// (provided the material shader has a property named "_BaseMap" anyway)
/// </summary>
public class CustomTexture : MonoBehaviour
{
    public string propertyName = "_MainTex";
    public Texture tex = null;
    [Tooltip("Should the texture overide all material indexes? (Requires the Render component to use more than 1 material)")]
    public bool updateWhole = false;
    [Tooltip("Material index to replace the texture (Requires updateWhole = true)")]
    public int matIndex = 0;
    MaterialPropertyBlock matProp;
    Renderer Rend;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rend = gameObject.GetComponent<Renderer>();
        if (Rend == null)
            Rend = gameObject.GetComponentInChildren<Renderer>();
        if (tex != null)
            UpdateMaterial(tex, matIndex);
    }

    private void OnValidate()
    {
        if (Rend == null)
            Rend = gameObject.GetComponent<Renderer>();
        if (Rend == null)
            Rend = gameObject.GetComponentInChildren<Renderer>();

        if (tex != null)
            UpdateMaterial(tex, matIndex);
        if (tex == null && matProp != null)
        {
            ClearMaterial(matIndex);
        }
    }

    public void UpdateMaterial(Texture texture, int index = 0)
    {
        matProp ??= new();
        matProp.SetTexture(propertyName, texture);

        if (Rend != null)
        {
            if (updateWhole)
                Rend.SetPropertyBlock(matProp);
            else if (index < Rend.sharedMaterials.Length)
                Rend.SetPropertyBlock(matProp, index);
            else
                Debug.LogError("Error: Material Index invalid");
        }
    }

    public void ClearMaterial(int index = -1)
    {
        if (Rend == null) return;
        // Clear everything if index is not specified
        if (index < 0)
            Rend.SetPropertyBlock(null);
        else if (index < Rend.sharedMaterials.Length)
            Rend.SetPropertyBlock(null, index);
        else
            Debug.LogError("Error: Material Index invalid");
    }
}
