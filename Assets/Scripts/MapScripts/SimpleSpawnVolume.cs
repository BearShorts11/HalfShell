using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawnVolume : MonoBehaviour
{
    // Code taken from the Bond SHMUP Chapter
    [Header("Inscribed")]
    
    public float boundsInset = 2f;   // Inset from the sides

    /*[Tooltip("Min/Max spawns per second - Minimum/Maximum spawn rate value for clamping the spawn per second value when it's modified by the difficulty settings")]
    public Vector2 minMaxSpawnPerSecond = new Vector2(0.5f, 1.5f);*/

    [Header("Dynamic")]
    [SerializeField] private Vector3 bounds;
    [SerializeField] public List<GameObject> enemies;
    
    private GameObject spawnVolume;
    private ArenaSurvivalGame survivalGame;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnVolume = this.gameObject;
        bounds = spawnVolume.GetComponent<Collider>().bounds.size;
        survivalGame = GameObject.FindAnyObjectByType<ArenaSurvivalGame>();
        if (survivalGame == null)
        {
            Debug.LogError("ERROR: No Survival Manager Object found!");
        }
    }

    public void SpawnSquad()
    {
        // Pick a random Enemy prefab to instantiate
        for (int ndx = 1; ndx< enemies.Count; ndx++){
            SpawnEnemy(enemies[ndx]);
        }
    }

    public void SpawnEnemy(GameObject enemyGO)
    {
        if (survivalGame == null || !enemyGO.TryGetComponent<Enemy>(out Enemy enemy)) return;

        GameObject go;
        // Set the initial position for the Enemy about to spawn
        Vector3 pos = Vector3.zero;

        pos.x = Random.Range(-(bounds.x * .5f) + boundsInset, (bounds.x * .5f) - boundsInset);
        pos.z = Random.Range(-(bounds.z * .5f) + boundsInset, (bounds.z * .5f) - boundsInset);
        go = Instantiate<GameObject>(enemyGO, this.gameObject.transform.position + pos, Quaternion.identity);
        enemy = go.GetComponent<Enemy>();
        enemy.Alert();
        survivalGame.IncreaseEnemyCount(enemy);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
    }
}
