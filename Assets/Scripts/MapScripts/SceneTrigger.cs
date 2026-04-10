using UnityEngine;

public class SceneTrigger : MonoBehaviour
{    
    public enum TriggerMode
    {
        LoadAndUnload,
        LoadOnly,
        UnloadOnly
    }

    [Header("Trigger Mode")]
    public TriggerMode triggerMode = TriggerMode.LoadAndUnload; //default load/unload

    [Header("Scenes")]
    //made strings to avoid scene index headaches with building
    [Tooltip("LOAD this scene when player enters trigger")]
    public string sceneToLoad;
    [Tooltip("UNLOAD this scene when player enters trigger")]
    public string sceneToUnload;

    [Header("Tag Filter")]
    public string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        //specifically using IsNullOrEmpty() because it catches both null and char-less entries
        //---------
        //if this doesn't fire it wont trigger the warning if the trigger mode is unload only
        if (triggerMode != TriggerMode.UnloadOnly && !string.IsNullOrEmpty(sceneToLoad))
        {
            SceneLoader.Instance.LoadScene(sceneToLoad);
        }
        else if (triggerMode != TriggerMode.UnloadOnly)
        {
            Debug.LogWarning("SceneTrigger - No scene to LOAD specified");
        }

        //if this doesn't fire it wont trigger the warning if the trigger mode is load only
        if (triggerMode != TriggerMode.LoadOnly && !string.IsNullOrEmpty(sceneToUnload))
        {
            SceneLoader.Instance.UnloadScene(sceneToUnload);
        }
        else if(triggerMode != TriggerMode.LoadOnly)
        {
            Debug.LogWarning("SceneTrigger - No scene to UNLOAD specified");
        }
    }
}
