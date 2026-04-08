using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

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
    [Tooltip("After this wave, item/object respawns will require more wave clears to respawn")]
    public int minWaveToStaggerItemSpawns = 2;
    [Tooltip("Cap the stagger count to prevent items from taking too long to spawn")]
    [Min(0)]public int maxItemSpawnStagger = 3;

    public Dictionary<int, SurvivalWave_Events> waveConditions = new();

    [Header("Dynamic")]
    [SerializeField] private int waveCount;
    [SerializeField] private List<SimpleSpawnVolume> spawnVolumes = new();
    [SerializeField] private List<SimpleSpawnVolume> activeSpawnVolumes = new();
    [SerializeField] private List<SimpleSpawner> mapSpawns = new();
    [SerializeField] private List<SimpleSpawner> activeMapSpawns = new();
    [SerializeField] private int wavesToRestock;
    [SerializeField] private int enemiesToWipe;
    [SerializeField] private List<Enemy> enemiesSpawned = new();
    [SerializeField] private float spawnRate;
    [SerializeField] private bool waveOver;
    private List<Enemy> enemiesDead = new();

    private int supplyStaggerCounter = 0;

    // Handle the enemy count?
    private UnityAction updateEnemyCounter;
    
    // Wave Fields 
    private SurvivalWave currentWave;
    private float nextSpawnTime;

    // Transition Time
    private float waitTime;
    private float nextWaitTime;

    UI_Message messageUI;
    UI_Wave waveCounter;

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

        messageUI = FindFirstObjectByType<UI_Message>();
        waveCounter = FindFirstObjectByType<UI_Wave>();

        Invoke(nameof(SetupGame), 1);
    }

    void SetupGame()
    {
        spawnVolumes = FindObjectsByType<SimpleSpawnVolume>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList<SimpleSpawnVolume>();
        
        mapSpawns = FindObjectsByType<SimpleSpawner>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList<SimpleSpawner>();

        updateEnemyCounter += DecreaseEnemyCount;

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

        RespawnItems();

        enemiesToWipe = waveCount < waves.Count ? currentWave.waveAmount : 8 * waveCount;

        //Debug.Log($"Wave {waveCount}!");
        if (waveCounter != null)
        {
            waveCounter.waveCount = waveCount;
            waveCounter.UpdateWave();
        }
        UI_Message_Update($"Wave {waveCount}, start!");
    }

    void EndWave()
    {
        if (waveConditions.ContainsKey(waveCount))
        {
            waveConditions[waveCount].RunClearEvents();
        }

        // If there are enemies spawned by the still alive when the wave ends
        if (enemiesSpawned.Count > 0)
        {
            foreach (Enemy enemy in enemiesSpawned)
            {
                if (enemy.Dead)
                    enemiesDead.Add(enemy);
            }
            foreach (Enemy deadenemy in enemiesDead)
            {
                deadenemy.TakeDamage(deadenemy.Health);
                if (enemiesSpawned.Contains(deadenemy))
                    enemiesSpawned.Remove(deadenemy);
            }
        }

        // WAVE COMPLETE!
        //Debug.Log("Wave Complete!");

        UI_Message_Update($"Wave {waveCount} complete!");
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
        //Debug.Log("Get Ready for the next Wave... (" + (waitTime) + " seconds)");
        UI_Message_Update("Prepare for the next wave...");
        Invoke(nameof(StartWave), waitTime);
    }

    SimpleSpawnVolume PickSpawnVolume()
    {
        activeSpawnVolumes.Clear();

        foreach (SimpleSpawnVolume volume in spawnVolumes)
        {
            if (volume.gameObject.activeInHierarchy)
                activeSpawnVolumes.Add(volume);
        }

        return activeSpawnVolumes[Random.Range(0, activeSpawnVolumes.Count)];
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
                if (Random.Range(0f, 1f) > 0.5f || currentWave.StrayEnemies.Count == 0 && currentWave.Squads.Count != 0)
                {
                    SurvivalWave_Squad squadSpawn = currentWave.Squads[Random.Range(0, currentWave.Squads.Count)];
                    spawnVol.enemies = squadSpawn.Enemies;
                    spawnVol.SpawnSquad();
                }
                else if (currentWave.StrayEnemies.Count != 0)
                    spawnVol.SpawnEnemy(currentWave.StrayEnemies[Random.Range(0, currentWave.StrayEnemies.Count)]);
                else
                    Debug.LogError($"ERROR: Wave configuration loaded but has no set Squads or Strays! Please check {currentWave.name}");
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
        enemy.OnDeath.AddListener(updateEnemyCounter);
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

    void UI_Message_Update(string message)
    {
        if (messageUI != null)
        {
            messageUI.SetMessage(message);
        }
    }

    void RespawnItems()
    {
        if (wavesToRestock <= 0)
        {
            foreach (SimpleSpawner mapSpawn in mapSpawns)
            {
                if (mapSpawn.gameObject.activeInHierarchy)
                {
                    mapSpawn.SpawnObject(mapSpawn.defaultObjectToSpawn);
                }
            }
            if (waveCount > minWaveToStaggerItemSpawns)
            {
                supplyStaggerCounter++;
                supplyStaggerCounter = Mathf.Clamp(supplyStaggerCounter, 0, maxItemSpawnStagger);
                wavesToRestock = supplyStaggerCounter;
            }
        }
        else
        {
            wavesToRestock--;
        }
    }

    // For map scripting or debugging
    public void ForceEndWave()
    {
        enemiesToWipe = 0;
        EndWave();
    }
}
