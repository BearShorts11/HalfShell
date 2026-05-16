using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;

// Not coded yet but will follow from this guide here
// (https://johnleonardfrench.music/terrain-footsteps-in-unity-how-to-detect-different-textures/)
// ADDENDUM: the link above is most likely obsolete since we're not using terrain any more and are instead resorting to using Polybrush instead.
//              Good thing I haven't gotten around to following that tutorial until now eh? -V, 4/20/2026

public class MaterialSurfaceTypeChecker : MonoBehaviour
{
    [SerializeField] private static ParticleSystem[] impactParticles;
    private static EventReference bulletImpactEvent;
    private static PARAMETER_ID impactSurfaceParamID;

    private static void Setup()
    {
        bulletImpactEvent = RuntimeManager.PathToEventReference("event:/Weapons/BulletImpacts/WeaponImpacts");

        impactParticles = new ParticleSystem[6];

        impactParticles[0] = Resources.Load<ParticleSystem>("PS_SmokePuff2");
        impactParticles[1] = Resources.Load<ParticleSystem>("PS_SmokePuff2");
        impactParticles[2] = Resources.Load<ParticleSystem>("PS_Impact_Wood");
        impactParticles[3] = Resources.Load<ParticleSystem>("PS_Impact_Concrete");
        impactParticles[4] = Resources.Load<ParticleSystem>("PS_Impact_Metal");
        impactParticles[5] = Resources.Load<ParticleSystem>("PS_BloodGush3");

        RuntimeManager.StudioSystem.lookupPath(bulletImpactEvent.Guid, out string eventPath);

        EventDescription eventDesc;
        RuntimeManager.StudioSystem.getEvent(eventPath, out eventDesc);

        PARAMETER_DESCRIPTION paramDesc;
        eventDesc.getParameterDescriptionByName("ImpactSurface", out paramDesc);

        impactSurfaceParamID = paramDesc.id;
    }

    private static Material CheckForProperty(Material[] materials)
    {
        foreach (Material sharedmat in materials)
            if (sharedmat != null)
                if (sharedmat.HasProperty("_SURFACETYPE"))
                    return sharedmat;
        return null;
    }

    public static int GetSurfaceType(Collider collider)
    {
        collider.gameObject.TryGetComponent<Renderer>(out Renderer renderer);
        Material mat = null;

        if (renderer != null)
            mat = CheckForProperty(renderer.sharedMaterials);
        
        if (mat == null || renderer == null) 
        {
            renderer = collider.gameObject.GetComponentInChildren<Renderer>(false); // Try looking for a render component in children if this is one of *those* game objects.
            if (renderer != null)
                mat = CheckForProperty(renderer.sharedMaterials);
        }
        if (mat == null || renderer == null) // this thing still null? look up the hiearchy instead
        { 
            renderer = collider.gameObject.GetComponentInParent<Renderer>(false);
            if (renderer != null)
                mat = CheckForProperty(renderer.sharedMaterials);
        }

        if (mat == null || renderer == null) // if this thing is still null, I give up -_-
            return 0;

        return GetSurfaceType(mat);
    }
    public static int GetSurfaceType(Material material)
    {
        if (material == null)
            return 0;

        if (!material.HasProperty("_SURFACETYPE"))
            return 0;

        return (int)material.GetFloat("_SURFACETYPE");
    }    

    public static void SpawnImpactParticle(int SurfaceType, Vector3 pos, Quaternion rot)
    {
        // Create a list of this for the first time
        if (impactParticles == null)
        {
            Setup();
        }

        ParticleSystem particle = Instantiate(impactParticles[SurfaceType], pos, rot);
        Destroy(particle, 5f);
    }

    public static void PlayImpactSound(RaycastHit hit)
    {
        if (bulletImpactEvent.IsNull) Setup();
        // Debug.Log($"Surface Value: {surfaceValue}");
        EventInstance impact = RuntimeManager.CreateInstance(bulletImpactEvent);

        // play sound where the bullet hit
        impact.set3DAttributes(RuntimeUtils.To3DAttributes(hit.point));

        int id = GetSurfaceType(hit.collider);
        if (hit.collider.gameObject.CompareTag("Enemy")) // Because it can't get the enemy's mesh renderer properly -_-
            id = 5;
            
        // set FMOD parameter for surface type
        impact.setParameterByID(impactSurfaceParamID, id);

        // impact.getParameterByID(impactSurfaceParamID, out float value);
        // Debug.Log("FMOD Parameter Set To: " + value);
        impact.start();
        impact.release();
    }
}
