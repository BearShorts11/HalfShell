using UnityEngine;

public class EjectedShell : MonoBehaviour
{
    [SerializeField] private float lifetime = 5.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Delete", lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Delete()
    {
        Destroy(this.gameObject);
    }
}
