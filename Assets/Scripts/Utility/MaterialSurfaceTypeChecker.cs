using System;
using UnityEngine;

// Not coded yet but will follow from this guide here
// (https://johnleonardfrench.music/terrain-footsteps-in-unity-how-to-detect-different-textures/)
// ADDENDUM: the link above is most likely obsolete since we're not using terrain any more and are instead resorting to using Polybrush instead.
//              Good thing I haven't gotten around to following that tutorial until now eh? -V, 4/20/2026
public class MaterialSurfaceTypeChecker : MonoBehaviour
{
    public static int GetSurfaceType(Material material)
    {
        if (material == null || !material.HasProperty("_SURFACETYPE"))
            return 0;
        return (int)material.GetFloat("_SURFACETYPE");
    }    
}
