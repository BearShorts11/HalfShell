/*  Viewmodel Handler (Based on code from this link: https://discussions.unity.com/t/how-to-move-a-skinned-mesh-renderer-over-to-another-model/797976)
 *  Handles the process of binding Game Objects that have the "Skinned Mesh Renderer" component
 *  and binds them to an other armature set for playing animations in sync. That is,
 *  if they both have the same armatures and are modelled around it.
 *  
 *  Optimization Todo: Can these SkinnedMeshRenderers be combined into a single one on runtime?
 */
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class ViewmodelHandler : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer[] targetSkin;
    [SerializeField] private Transform rootBone;

    private void Awake()
    {
        if (!rootBone)
        {
            // RootBone is not a thing, what.
            //rootBone = FindFirstObjectByType<RootBone>().transform;
            return;
        }

        Dictionary<string, Transform> boneDictionary = new Dictionary<string, Transform>();
        Transform[] rootBoneChildren = rootBone.GetComponentsInChildren<Transform>();
        foreach (Transform child in rootBoneChildren)
        {
            boneDictionary[child.name] = child;
        }

        for (int j = 0; j < targetSkin.Length; j++)
        {
            Transform[] newBones = new Transform[targetSkin[j].bones.Length];
            for (int i = 0; i < targetSkin[j].bones.Length; i++)
            {
                if (boneDictionary.TryGetValue(targetSkin[j].bones[i].name, out Transform newBone))
                {
                    newBones[i] = newBone;
                }
            }
            targetSkin[j].bones = newBones;
        }
    }
}
