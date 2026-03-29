using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
public class ArenaSurvivalGame : MonoBehaviour
{
    static public ArenaSurvivalGame Instance;

    [Header("Inscribed")]
    public List<SurvivalWave> waves;

    [Tooltip("Time delay to let the wave end. Adds onto the wave transition time set in the Wave Scriptable Object")]
    public int preparationTime = 5;
    [Tooltip("Default rate to use when there are no more defined waves to go through")]
    public float endlessWaveSpawnRate = 0.3f;
    [Tooltip("Default pool of enemies to draw when there are no waves configured")]
    public List<GameObject> defaultEnemies;
    [Header("Wave Events - Scripted Events executed on a specific wave")]
    public SurvivalWave_Events[] waveEvents;
    [Tooltip("How many enemies can exist at a single time on the map")]
    public int maxEnemiesAtOnce = 32;

    public Dictionary<int, SurvivalWave_Events> waveConditions = new();

    [Header("Dynamic")]
    [SerializeField] private int waveCount;
    [SerializeField] private List<SimpleSpawnVolume> spawnVolumes;
    [SerializeField] private int enemiesToWipe;
    [SerializeField] private List<Enemy> enemiesSpawned = new();
    [SerializeField] private float spawnRate;
    [SerializeField] private bool waveOver;
    private List<Enemy> enemiesDead;

    // Handle the enemy count?
    private UnityAction updateEnemyCounter;
    
    // Wave Fields 
    private SurvivalWave currentWave;
    private float nextSpawnTime;

    // Transition Time
    private float waitTime;
    private float nextWaitTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;

        if (Instance != this)
        {
            Debug.LogError("ERROR: More than one instace of an GameObject using this script is in the scene! Delete that spare object/component and restart the scene.");
            return; 
        }

        for (int i = 0; i < waveEvents.Length; i++)
        {
            waveConditions.Add(waveEvents[i].waveIndex, waveEvents[i]);
        }

        Invoke(nameof(SetupGame), 1);
    }

    void SetupGame()
    {
        spawnVolumes = FindObjectsByType<SimpleSpawnVolume>(FindObjectsSortMode.None).ToList<SimpleSpawnVolume>();

        updateEnemyCounter += DecreaseEnemyCount;
        Enemy.DeathAlert.AddListener(updateEnemyCounter);

        Invoke(nameof(StartWave), 5);
    }

    // Update is called once per frame
    void Update()
    {
        waveOver = enemiesToWipe <= 0;
        if (waveOver) return;

        WaveUpdate();
    }

    void StartWave()
    {
        if (WaveConfigured())
            currentWave = waves[waveCount];

        if (waveConditions.ContainsKey(waveCount))
        {
            waveConditions[waveCount].RunStartEvents();
        }

        enemiesToWipe = waveCount < waves.Count ? currentWave.waveAmount : 8 * waveCount;

        Debug.Log($"Wave {waveCount}!");
    }

    void EndWave()
    {
        if (waveConditions.ContainsKey(waveCount))
        {
            waveConditions[waveCount].RunClearEvents();
        }
        // WAVE COMPLETE!
        Debug.Log("Wave Complete!");
        Invoke(nameof(SetUpNextWave), preparationTime);
    }

    void SetUpNextWave()
    {
        waitTime = currentWave == null ? preparationTime : currentWave.wavetransitionTime;
        IncrementWave();
        foreach (Enemy enemy in enemiesDead)
        {
            Destroy(enemy.gameObject);
        }
        enemiesDead.Clear();
        Debug.Log("Get Ready for the next Wave... (" + (waitTime) + " seconds)");
        Invoke(nameof(StartWave), waitTime);
    }

    SimpleSpawnVolume PickSpawnVolume()
    {
        List<SimpleSpawnVolume> mapSpawnVolumes = new();

        foreach (SimpleSpawnVolume volume in spawnVolumes)
        {
            if (volume.gameObject.activeInHierarchy)
                mapSpawnVolumes.Add(volume);
        }

        return mapSpawnVolumes[Random.Range(0, mapSpawnVolumes.Count)];
    }
    bool WaveConfigured()
    {
        return waveCount < waves.Count;
    }

    bool AllEnemiesSpawned()
    {
        return enemiesSpawned.Count >= enemiesToWipe || enemiesSpawned.Count > maxEnemiesAtOnce;
    }

    void WaveUpdate()
    {
        if (nextSpawnTime < Time.fixedTime && !AllEnemiesSpawned())
        {
            nextSpawnTime = Time.fixedTime + currentWave.spawnRate;
            SimpleSpawnVolume spawnVol = PickSpawnVolume();
            if (WaveConfigured())
            {
                // Spawn a squad or a stray enemy
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    SurvivalWave_Squad squadSpawn = currentWave.Squads[Random.Range(0, currentWave.Squads.Count)];
                    spawnVol.enemies = squadSpawn.Enemies;
                    spawnVol.SpawnSquad();
                }
                else
                    spawnVol.SpawnEnemy(currentWave.StrayEnemies[Random.Range(0, currentWave.StrayEnemies.Count)]);
            }
            else
                spawnVol.SpawnEnemy(defaultEnemies[Random.Range(0, defaultEnemies.Count)]);

            // Are there more enemies that spawned more than the required kill count (Via Squad Spawn)?
            if (enemiesSpawned.Count > enemiesToWipe)
                enemiesToWipe = enemiesSpawned.Count;
        }
    }

    public void IncreaseEnemyCount(Enemy enemy)
    {
        enemiesSpawned.Add(enemy);
        enemy.Alert();
    }

    private void DecreaseEnemyCount()
    {
        if (enemiesToWipe <= 0) return;

        enemiesToWipe--;
        if (enemiesToWipe <= 0)
        {
            EndWave();
        }

        UpdateEnemiesSpawnedList();
    }

    private void UpdateEnemiesSpawnedList()
    {
        foreach (Enemy enemy in enemiesSpawned)
        {
            if (enemy.Dead) 
                enemiesDead.Add(enemy);
        }
        foreach(Enemy deadenemy in enemiesDead)
        {
            if (enemiesSpawned.Contains(deadenemy))
                enemiesSpawned.Remove(deadenemy);
        }
    }

    void IncrementWave()
    {
        waveCount++;
    }
}
