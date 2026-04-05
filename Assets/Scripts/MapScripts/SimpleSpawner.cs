using UnityEngine;

// Script component made for Arena Mode
// Mainly used for spawning items every wave (if needed)
public class SimpleSpawner : MonoBehaviour
{
    public GameObject defaultObjectToSpawn;
    GameObject spawnedObject;

    public void SpawnObject()
    {
        SpawnObject(defaultObjectToSpawn);
    }

    public void SpawnObject(GameObject obj)
    {
        if (spawnedObject == null) {
            spawnedObject = Instantiate(obj, this.transform.position, Quaternion.identity);
        }
    }
}
