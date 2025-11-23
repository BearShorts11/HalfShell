using UnityEngine;

public class GibletParticles : MonoBehaviour
{
    [SerializeField] private float lifetime = 5.0f;
    public GameObject[] gibModels;
    [Tooltip("Amount of Gibs to spawn")]
    public Vector2Int minMaxAmount = new Vector2Int(3, 12);
    [Tooltip("Force to apply when spawned")]
    public Vector2 minMaxForce = new Vector2(4, 8);
    private int gibsAmount;
    private Vector3 rotation;
    private Vector3 force;
    private Rigidbody gibRB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject go;
        gibsAmount = Random.Range(minMaxAmount.x, minMaxAmount.y);
        if (gibsAmount > 0 && gibModels.Length > 0)
        {
            for (int i = 0; i < gibsAmount; i++)
            {
                rotation.x = Random.Range(-360f, 360f);
                rotation.y = Random.Range(-360f, 360f);
                rotation.z = Random.Range(-360f, 360f);
                go = Instantiate(gibModels[Random.Range(0,gibModels.Length)],this.gameObject.transform.position, Quaternion.Euler(rotation));
                gibRB = go.GetComponent<Rigidbody>();
                if (gibRB == null) { Debug.LogError($"Rigid Body not found for {go}!"); continue; }
                force.x = Random.Range(Random.Range(-minMaxForce.x, minMaxForce.x), Random.Range(-minMaxForce.y,minMaxForce.y));
                force.y = Random.Range(minMaxForce.x, minMaxForce.y) * 2f;
                //force.y *= gibRB.mass <= 1 ? 1.2f : gibRB.mass;
                force.z = Random.Range(Random.Range(-minMaxForce.x, minMaxForce.x), Random.Range(-minMaxForce.y, minMaxForce.y));
                gibRB.AddForce(force, ForceMode.VelocityChange);
                gibRB.AddTorque(rotation*Random.Range(1,4));
                Destroy(go, lifetime);
            }
        }
        Destroy(this.gameObject, lifetime + 1);
    }
}
