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

    //Go to line 69
    private Scene _currentScene;
    [SerializeField] public Transform Spawnpoint;

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
        Spawnpoint = GameObject.Find("Spawnpoint").transform;

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
        //TODO: disable "GENERAL"

        //load scene with a BG thread without replacing current scene
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //telling unity to activate scene ASAP
        op.allowSceneActivation = true;
        //Pause until async op is done
        yield return op;
        //trackk scene to avoid dupe loading
        _loadedScenes.Add(sceneName);

        //COMMENTED TO AVOID RELOAD CONFLICTS
        _currentScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(_currentScene);

        FindFirstObjectByType<PlayerBehavior>().gameObject.SetActive(false);
        FindFirstObjectByType<PlayerUI>().gameObject.SetActive(false);
        //TODO: Enable "GENERAL"

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

    public Scene GetCurrentScene() => _currentScene;
}
