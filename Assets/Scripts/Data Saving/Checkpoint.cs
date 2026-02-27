using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class Checkpoint : MonoBehaviour
{
    public static UnityEvent SaveGame = new UnityEvent();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        { 
            SaveGame.Invoke();
        }
    }
}
