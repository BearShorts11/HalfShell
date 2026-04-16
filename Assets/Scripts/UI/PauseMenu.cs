//using UnityEditor;
//using UnityEditor.SearchService;
using Assets.Scripts;
using FMOD.Studio;
using FMODUnity;
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
    public GameObject LoadingSceneText;

    // Volume Editing Properties
    private Bus masterBus;
    private Bus soundBus;
    private Bus uiBus; // Shares the same volume as soundBus, but should not be effected by the timescaling effect in FMOD.
    private Bus musicBus;
    private Bus dialogueBus;
    public Slider masterVolumeSlider; 
    public Slider sfxVolumeSlider; 
    public Slider musicVolumeSlider; 
    public Slider dialogueVolumeSlider;
    public TextMeshProUGUI mastervol_val_txt;
    public TextMeshProUGUI sfxvol_val_txt;
    public TextMeshProUGUI musicvol_val_txt;
    public TextMeshProUGUI dialoguevol_val_txt;
    private float masterVolume = 1;
    private float sfxVolume = 1;
    private float musicVolume = 1;
    private float dialogueVolume = 1;
    [Range(0,1f)] public const float DEFAULT_MASTER_VOLUME = 1;
    [Range(0,1f)] public const float DEFAULT_SFX_VOLUME = 1;
    [Range(0,1f)] public const float DEFAULT_MUSIC_VOLUME = 1;
    [Range(0,1f)] public const float DEFAULT_DIALOGUE_VOLUME = 1;
    public const string MASTER_VOLUME_KEY = "FMOD_MASTER_VOLUME";
    public const string SFX_VOLUME_KEY = "FMOD_SFX_VOLUME";
    public const string MUSIC_VOLUME_KEY = "FMOD_MUSIC_VOLUME";
    public const string DIALOGUE_VOLUME_KEY = "FMOD_DIALOGUE_VOLUME";

    string JsonFilePath = "Assets/JsonFiles/Settings/PlayerSettings.txt";

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
        SensitivitySlider.onValueChanged.AddListener(delegate { SensitivityValueChange();  });

        masterVolumeSlider.onValueChanged.AddListener(delegate      { UpdateVolumeSettings();  });
        sfxVolumeSlider.onValueChanged.AddListener(delegate         { UpdateVolumeSettings();  });
        musicVolumeSlider.onValueChanged.AddListener(delegate       { UpdateVolumeSettings();  });
        dialogueVolumeSlider.onValueChanged.AddListener(delegate    { UpdateVolumeSettings();  });
        
        // These are all Sound Groups that can be edited for mastering
        masterBus = RuntimeManager.GetBus("bus:/");
        soundBus = RuntimeManager.GetBus("bus:/SFX");
        uiBus = RuntimeManager.GetBus("bus:/UI");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        dialogueBus = RuntimeManager.GetBus("bus:/Dialogue");
        // For each bus that has their volume updated
        // They can be run through a method that updates them
        // Each that have their volume adjusted should be adjusted with this
        // code: [groupName]Bus.SetVolume(floatValue/float property);

        SensitivitySlider.value = player.UpdateSensitivity();
        SensitivityValueChange();
        FOVSlider.value = player.UpdateFOV();
        FOVValueChange();

        if (PlayerPrefs.HasKey(MASTER_VOLUME_KEY))
            masterVolumeSlider.value    = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY);
        if (PlayerPrefs.HasKey(SFX_VOLUME_KEY))
            sfxVolumeSlider.value       = PlayerPrefs.GetFloat(SFX_VOLUME_KEY);
        if (PlayerPrefs.HasKey(MUSIC_VOLUME_KEY))
            musicVolumeSlider.value     = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
        if (PlayerPrefs.HasKey(DIALOGUE_VOLUME_KEY))
            dialogueVolumeSlider.value  = PlayerPrefs.GetFloat(DIALOGUE_VOLUME_KEY);

        LoadVolumeSettings();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu.activeSelf)
            { 
                settingsMenu.SetActive(false);
                pauseMenu.SetActive(true);
                return;
            }
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
        player.Revive();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ResetSceneFromCheckpoint()
    {
        player.Revive();
        SaveLoadSystem system = FindFirstObjectByType<SaveLoadSystem>();
        system.LoadGame(system.gameData.Name);

        //undo death actions

        paused = false;
        deathMenu.SetActive(false);
        pauseMenu.SetActive(false);
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
        LoadingSceneText.SetActive(true);
        SceneManager.LoadScene(selecteedScene);
        paused = false;
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

        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, DEFAULT_MASTER_VOLUME);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, DEFAULT_SFX_VOLUME);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, DEFAULT_MUSIC_VOLUME);
        PlayerPrefs.SetFloat(DIALOGUE_VOLUME_KEY, DEFAULT_DIALOGUE_VOLUME);

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

        SaveVolumeSettings();
    }

    private void SaveUpdateSettings()
    {
        PlayerPrefs.Save();
        player.UpdateSensitivity();
        player.UpdateFOV();
        FOV_val_txt.text = $"{FOVSlider.value}";


        //save out to json
    }

    private void LoadVolumeSettings()
    {
        masterBus.setVolume(masterVolume);
        soundBus.setVolume(sfxVolume);
        uiBus.setVolume(sfxVolume);
        musicBus.setVolume(musicVolume);
        dialogueBus.setVolume(dialogueVolume);
    }

    private void UpdateVolumeSettings()
    {
        masterVolume =      masterVolumeSlider.value;
        sfxVolume =         sfxVolumeSlider.value;
        musicVolume =       musicVolumeSlider.value;
        dialogueVolume =    dialogueVolumeSlider.value;

        mastervol_val_txt   .text = masterVolume.ToString("#.00");
        sfxvol_val_txt      .text = sfxVolume.ToString("#.00");
        musicvol_val_txt    .text = musicVolume.ToString("#.00");
        dialoguevol_val_txt .text = dialogueVolume.ToString("#.00");

        LoadVolumeSettings();
        SaveVolumeSettings(); // Shouldn't really put it here but since there is no apply button, but I guess this will do.
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, masterVolume);
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxVolume);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicVolume);
        PlayerPrefs.SetFloat(DIALOGUE_VOLUME_KEY, dialogueVolume);

        LoadVolumeSettings();
    }

    private void FOVValueChange()
    {
        FOV_val_txt.text = $"{FOVSlider.value}";

        float FOVvalue = FOVSlider.value;
        PlayerPrefs.SetFloat(PlayerBehavior.FOV_KEY, FOVvalue);
        SaveUpdateSettings();
    }

    private void SensitivityValueChange()
    {
        float sensitivityModifier = SensitivitySlider.value;
        PlayerPrefs.SetFloat(PlayerBehavior.SENSITIVITY_KEY, sensitivityModifier);
        //FOV_val_txt.text = $"{FOVSlider.value}";
        SaveUpdateSettings();
    }
}
