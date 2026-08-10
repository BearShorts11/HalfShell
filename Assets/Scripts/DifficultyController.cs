using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyController : MonoBehaviour
{
    public enum Difficulty
    { 
        EASY = 0, MEDIUM = 1, HARD = 2
    }
    public Difficulty difficulty = Difficulty.MEDIUM;

    [SerializeField] GameObject DifficultySelectionScreen;
    [SerializeField] GameObject ReadyButton;
    [SerializeField] GameObject EasyIndicator;
    [SerializeField] GameObject MedIndicator;
    [SerializeField] GameObject HardIndicator;

    [SerializeField] GameObject LoadingText;

    public static int startSceneIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //SetMediumDifficulty();
        if (LoadingText != null) LoadingText.SetActive(false);

        //skips has key and set difficulty checks if in the title screen
        //(player will select a difficulty there, no need to do it before then + it breaks if done before selected by player)
        if (SceneManager.GetActiveScene().buildIndex == 0) return;

        if (PlayerPrefs.HasKey("DIFFICULTY"))
        { 
            difficulty = (Difficulty)PlayerPrefs.GetInt("DIFFICULTY");
            switch (difficulty)
            { 
                case Difficulty.EASY:
                    SetEasyDifficulty();
                    break;
                case Difficulty.MEDIUM:
                    SetMediumDifficulty();
                    break;
                case Difficulty.HARD:
                    SetHardDifficulty();
                    break;
            }
        }
        else SetMediumDifficulty();
    }

    [ContextMenu("Set Easy")]
    public void SetEasyDifficulty()
    { 
        difficulty = Difficulty.EASY;
        PlayerPrefs.SetInt("DIFFICULTY", (int)difficulty);
        IPickup.regainMultiplier = 2;
        Enemy.DamageMultiplier = 0.5f;

        Slug.MaxHolding = 20;
        Incindiary.MaxHolding = 15;
        //FindFirstObjectByType<PlayerShooting>().CheckAboveMaxAmmo();

        if (DifficultySelectionScreen != null) DifficultySelectionScreen.SetActive(false);
        //if (ReadyButton != null) ReadyButton.SetActive(true);
        if(SceneManager.GetActiveScene().buildIndex == 0) TitleScreenUI.StartGameFromDifficulty(startSceneIndex);

        //ToggleIndicatorImage();
        if(LoadingText != null) LoadingText.SetActive(true);
    }

    [ContextMenu("Set Med")]
    public void SetMediumDifficulty()
    { 
        difficulty = Difficulty.MEDIUM;
        PlayerPrefs.SetInt("DIFFICULTY", (int)difficulty);
        IPickup.regainMultiplier = 1;
        Enemy.DamageMultiplier = 1;

        Slug.MaxHolding = 15;
        Incindiary.MaxHolding = 10;
        //FindFirstObjectByType<PlayerShooting>().CheckAboveMaxAmmo();

        if (DifficultySelectionScreen != null) DifficultySelectionScreen.SetActive(false);
        //if (ReadyButton != null) ReadyButton.SetActive(true);
        if (SceneManager.GetActiveScene().buildIndex == 0) TitleScreenUI.StartGameFromDifficulty(startSceneIndex);

        //ToggleIndicatorImage();
        if (LoadingText != null) LoadingText.SetActive(true);
    }

    [ContextMenu("Set Hard")]
    public void SetHardDifficulty()
    { 
        difficulty = Difficulty.HARD;
        PlayerPrefs.SetInt("DIFFICULTY", (int)difficulty);
        IPickup.regainMultiplier = 0.5f;
        Enemy.DamageMultiplier = 1.5f;

        Slug.MaxHolding = 10;
        Incindiary.MaxHolding = 5;
        //FindFirstObjectByType<PlayerShooting>().CheckAboveMaxAmmo();

        if (DifficultySelectionScreen != null) DifficultySelectionScreen.SetActive(false);
        //if (ReadyButton != null) ReadyButton.SetActive(true);
        if (SceneManager.GetActiveScene().buildIndex == 0) TitleScreenUI.StartGameFromDifficulty(startSceneIndex);

        //ToggleIndicatorImage();
        if (LoadingText != null) LoadingText.SetActive(true);
    }

    private void ToggleIndicatorImage()
    {
        switch (difficulty)
        { 
            case Difficulty.HARD:
                HardIndicator.SetActive(true);
                MedIndicator.SetActive(false);
                EasyIndicator.SetActive(false);
                break;
            case Difficulty.MEDIUM:
                HardIndicator.SetActive(false);
                MedIndicator.SetActive(true);
                EasyIndicator.SetActive(false);
                break;
            case Difficulty.EASY:
                HardIndicator.SetActive(false);
                MedIndicator.SetActive(false);
                EasyIndicator.SetActive(true);
                break;
        }
    }
}
