using UnityEngine;

public class DifficultyController : MonoBehaviour
{
    public enum Difficulty
    { 
        EASY = 0, MEDIUM = 1, HARD = 2
    }
    public Difficulty difficulty = Difficulty.MEDIUM;   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetMediumDifficulty();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Set Easy")]
    public void SetEasyDifficulty()
    { 
        difficulty = Difficulty.EASY;
        IPickup.regainMultiplier = 2;
        Enemy.DamageMultiplier = 0.5f;

        Slug.MaxHolding = 20;
        Incindiary.MaxHolding = 15;
        FindFirstObjectByType<PlayerShooting>().CheckAboveMaxAmmo();
    }
    [ContextMenu("Set Med")]
    public void SetMediumDifficulty()
    { 
        difficulty = Difficulty.MEDIUM;
        IPickup.regainMultiplier = 1;
        Enemy.DamageMultiplier = 1;

        Slug.MaxHolding = 15;
        Incindiary.MaxHolding = 10;
        FindFirstObjectByType<PlayerShooting>().CheckAboveMaxAmmo();
    }
    [ContextMenu("Set Hard")]
    public void SetHardDifficulty()
    { 
        difficulty = Difficulty.HARD;
        IPickup.regainMultiplier = 0.5f;
        Enemy.DamageMultiplier = 2;

        Slug.MaxHolding = 10;
        Incindiary.MaxHolding = 5;
        FindFirstObjectByType<PlayerShooting>().CheckAboveMaxAmmo();
    }
}
