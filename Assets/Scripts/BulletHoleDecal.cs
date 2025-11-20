using UnityEngine;

public class BulletHoleDecal : MonoBehaviour
{
    Transform parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(this.gameObject, 30f);
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        //kill if parent dies
        if (parent is null) Destroy(this.gameObject);
    }
}
