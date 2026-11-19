using UnityEngine;

// Psuedo Skybox Fog Material controller for Mesh Rendered GameObject
public class FogCatcher : MonoBehaviour
{
    [TextArea(4,8)] [SerializeField] private string readme;

    [Header("Fog Height (-1: Highest, 1: Lowest)")]
    [Range(-1,1)] public float FogHeight = 0f;

    MaterialPropertyBlock propBlock;
    Renderer rend;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyFogHeight();
    }

    private void OnValidate()
    {
        ApplyFogHeight();
    }
    private void ApplyFogHeight()
    {
        propBlock ??= new();
        if (!rend)
            rend = GetComponent<Renderer>();

        propBlock.SetFloat("_Height", FogHeight);
        rend.SetPropertyBlock(propBlock);
    }

    public void SetNewFogHeight(float newFogHeight)
    {
        newFogHeight = Mathf.Clamp(newFogHeight, -1, 1);
        if (FogHeight != newFogHeight)
        {
            FogHeight = newFogHeight;

            ApplyFogHeight();
        }
    }
}
