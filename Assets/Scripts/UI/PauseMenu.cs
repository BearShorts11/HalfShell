using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public GameObject sceneSelectMenu;
    private PlayerBehavior player;

    public enum Scene
    {
        MockUp, //0
        SampleScene, //1
        TestArena, //2
    }
    public Scene currentScene;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerBehavior>();
        if (paused) { Pause(); }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if (player.Health <= 0)
        {
            Death();
        }
    }

    public void Pause()
    {
        switch (paused)
        {
            case false:
                PlayerBehavior.UnlockCursor();
                PlayerBehavior.PauseTime();
                pauseMenu.SetActive(true);
                paused = true;
                break;
            case true:
                PlayerBehavior.LockCursor();
                PlayerBehavior.ResumeTime();
                pauseMenu.SetActive(false);
                paused = false;
                break;
        }
    }

    public void ReloadCurrentScene()
    {
        paused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Death()
    {
        paused = true;
        PlayerBehavior.UnlockCursor();
        PlayerBehavior.PauseTime();
        deathMenu.SetActive(true);
    }

    public void SceneSelectMenu()
    {
        sceneSelectMenu.SetActive(true);
    }

    public void BackButton(GameObject screen)
    {
        screen.SetActive(false);
    }

    public void LoadSelectedScene(int selecteedScene)
    {
        SceneManager.LoadScene(selecteedScene);
    }
}
