using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Tooltip("Scene you want to be loaded by default. Choose your starting area.")]
    public string initialScene; // no pre-placing for tracking, sorry :(

    //tracking what scene is currently loaded
    private List<string> _loadedScenes = new List<string>();

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if(!string.IsNullOrEmpty(initialScene))
        {
            LoadScene(initialScene);
        }
        else 
        {
            Debug.LogWarning("SceneLoader - No intial scene specified");
        }
    }
    public void LoadScene(string sceneName)
    {
        if (!_loadedScenes.Contains(sceneName))
        {
            StartCoroutine(LoadRoutine(sceneName));
        }
    }

    public void UnloadScene(string sceneName)
    {
        if(_loadedScenes.Contains(sceneName)) 
        {
            StartCoroutine(UnloadRoutine(sceneName));
        }
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        //load scene with a BG thread without replacing current scene
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //telling unity to activate scene ASAP
        op.allowSceneActivation = true;
        //Pause until async op is done
        yield return op;
        //trackk scene to avoid dupe loading
        _loadedScenes.Add(sceneName);
        Debug.Log($"SceneLoader Loaded: {sceneName}");
    }

    private IEnumerator UnloadRoutine(string sceneName)
    {
        // unloading scene with BG thread
        //destroys all GO's in that scene
        AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);

        //pausing until op is done pt.2
        yield return op;
        //removing scene from list so it can be loaded again
        _loadedScenes.Remove(sceneName);
        Debug.Log($"SceneLoader Unlaoded: {sceneName}");
    }
}
