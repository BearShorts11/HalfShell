using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    private PlayerBehavior player;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<PlayerBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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

        if (player.Health <= 0)
        {
            Death();
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
}
