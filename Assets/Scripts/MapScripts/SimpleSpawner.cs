using UnityEngine;

// Script component made for Arena Mode
// Mainly used for spawning items every wave (if needed)
public class SimpleSpawner : MonoBehaviour
{
    public GameObject defaultObjectToSpawn;
    [Tooltip("Requires Child GameObject to be set and defaultObjectToSpawn be empty \nShould the child object be active when the scene starts or should it behave like defaultObjectToSpawn if it was set to a prefab")]
    public bool bActiveOnStart = false;
    GameObject spawnedObject;

    private void Start()
    {
        // Child object as the default object to spawn: In the case of having a prefab that is modular, setting a modified prefab as a child to this game object will be set as the default object to spawn.
        if (defaultObjectToSpawn == null && transform.childCount > 0)
        {
            defaultObjectToSpawn = transform.GetChild(0).gameObject;
            if (bActiveOnStart)
            {
                spawnedObject = defaultObjectToSpawn;
                return;
            }
            if (defaultObjectToSpawn.activeSelf)
            {
                defaultObjectToSpawn.SetActive(false);
            }
        }
    }

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
