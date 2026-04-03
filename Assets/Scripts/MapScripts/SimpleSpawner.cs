using UnityEngine;

// Script made for Simple Triggers
public class SimpleSpawner : MonoBehaviour
{
    public void SpawnObject(GameObject obj)
    {
        Instantiate(obj, this.transform.position, Quaternion.identity);
    }
}
