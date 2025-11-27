//using UnityEditor;
//using UnityEditor.SearchService;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool paused = false;
    public GameObject pauseMenu;
    public GameObject deathMenu;
    public GameObject sceneSelectMenu;
    public GameObject settingsMenu;
    private PlayerBehavior player;

    public Slider SensitivitySlider; 
    public Slider FOVSlider;
    public TextMeshProUGUI FOV_val_txt;

    string JsonFilePath = "Assets/JsonFiles/PlayerSettings.txt";

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

        FOVSlider.onValueChanged.AddListener(delegate { FOVValueChange();  });
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
        if (screen.name == "Settings Menu")
        {
            pauseMenu.SetActive(true);
        }
    }

    public void LoadSelectedScene(int selecteedScene)
    {
        SceneManager.LoadScene(selecteedScene);
    }

    public void OpenSettings()
    { 
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void ResetSettings()
    {
        Debug.Log("reset settings");
        PlayerPrefs.SetFloat(PlayerBehavior.SENSITIVITY_KEY, PlayerBehavior.DEFAULT_SENSITIVITY_MOD);
        PlayerPrefs.SetFloat(PlayerBehavior.FOV_KEY, PlayerBehavior.DEFAULT_FOV_VALUE);

        SensitivitySlider.value = PlayerBehavior.DEFAULT_SENSITIVITY_MOD;
        FOVSlider.value = PlayerBehavior.DEFAULT_FOV_VALUE;

        SaveUpdateSettings();
    }

    public void ApplySettings()
    {
        //get value from slider element
        //set to PlayerPrefs;
        float sensitivityModifier = SensitivitySlider.value;
        PlayerPrefs.SetFloat(PlayerBehavior.SENSITIVITY_KEY, sensitivityModifier);

        float FOVvalue = FOVSlider.value;
        PlayerPrefs.SetFloat(PlayerBehavior.FOV_KEY, FOVvalue);

        SaveUpdateSettings();
    }

    private void SaveUpdateSettings()
    {
        PlayerPrefs.Save();
        player.UpdateSensitivity();
        player.UpdateFOV();
        FOV_val_txt.text = $"{FOVSlider.value}";
    }

    private void FOVValueChange()
    {
        FOV_val_txt.text = $"{FOVSlider.value}";

    }
}
