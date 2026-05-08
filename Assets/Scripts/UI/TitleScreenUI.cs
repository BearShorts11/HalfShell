using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] GameObject DifficultySelect;


    [SerializeField] GameObject StartButton;
    [SerializeField] GameObject ExitButton;
    [SerializeField] GameObject Logo;


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
            DifficultyController.startSceneIndex = sceneNumber;

            StartButton.SetActive(false); ExitButton.SetActive(false); Logo.SetActive(false);
        }
        else 
        { 
            SceneManager.UnloadSceneAsync(currentScene);
            SceneManager.LoadSceneAsync(sceneNumber);
        }
    }

    public static void StartFromDifficulty(int sceneNumber)
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
