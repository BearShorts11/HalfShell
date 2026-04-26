using UnityEngine;

// Script component made for Arena Mode
// Mainly used for spawning items every wave (if needed)
public class SimpleSpawner : MonoBehaviour
{
    public GameObject defaultObjectToSpawn;
    [Tooltip("Should the child object be active when the scene starts or should it behave like defaultObjectToSpawn if it was set to a prefab")]
    public bool bActiveOnStart = false;
    private GameObject sceneObject;
    GameObject spawnedObject;

    private void Start()
    {
        // Child object as the default object to spawn: In the case of having a prefab that is modular, setting a modified prefab as a child to this game object will be set as the default object to spawn.
        if (defaultObjectToSpawn == null && transform.childCount > 0)
        {
            sceneObject = transform.GetChild(0).gameObject;
            defaultObjectToSpawn = sceneObject;
            if (sceneObject.activeSelf)
            {
                sceneObject.SetActive(false);
            }
        }
        if (bActiveOnStart)
            SpawnObject();
    }

    public void SpawnObject()
    {
        SpawnObject(defaultObjectToSpawn);
    }

    public void SpawnObject(GameObject obj)
    {
        if (spawnedObject == null) {
            ToggleSceneObject();
            spawnedObject = Instantiate(obj, this.transform.position, Quaternion.identity);
            spawnedObject.SetActive(true);
            ToggleSceneObject();
        }
    }

    private void ToggleSceneObject()
    {
        if (sceneObject != null)
        {
            sceneObject.SetActive(!sceneObject.activeSelf);
        }
    }
}
