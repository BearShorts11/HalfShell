using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    public void DestorySelf()
    { 
        Destroy(gameObject);
    }

    public void DisableSelf()
    { 
        gameObject.SetActive(false);
    }
}
