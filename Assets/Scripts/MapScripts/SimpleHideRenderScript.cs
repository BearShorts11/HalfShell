using UnityEngine;

// Basically, do not render this game object when the scene is running. This is mainly applicable to map editor objects like Clip brushes and Trigger volumes. -V
public class SimpleHideRenderScript : MonoBehaviour
{
    MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null )
            meshRenderer.enabled = false;
    }
}
