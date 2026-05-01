using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] GameObject DifficultySelect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGameButton(int sceneNumber)
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (DifficultySelect != null)
        {
            DifficultySelect.SetActive(true);
        }
        else 
        { 
            SceneManager.UnloadSceneAsync(currentScene);
            SceneManager.LoadSceneAsync(sceneNumber);
        }
    }

    public void StartFromReady(int sceneNumber)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.LoadSceneAsync(sceneNumber);
    }

    public void ExitGameButton()
    {
        Application.Quit();
    }
}
