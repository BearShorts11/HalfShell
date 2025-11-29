using UnityEngine;

public class GibletParticles : MonoBehaviour
{
    [SerializeField] private float lifetime = 12.0f;
    [SerializeField] private float giblifetime = 9.0f;
    public Mesh[] randomGibModels;
    public GameObject[] gibObjects;
    public Material[] gibMaterials;
    [Tooltip("Minimal amount of Gibs to spawn")]
    public int minAmount = 3;
    [Tooltip("Force to apply when spawned")]
    public Vector2 minMaxForce = new Vector2(4, 8);
    private int gibsAmount;
    private Vector3 rotation;
    private Vector3 force;
    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    private Renderer rend;
    private Rigidbody gibRB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject go;
        this.gameObject.transform.parent = null;
        if (gibObjects.Length > 0)
        {
            gibsAmount = Random.Range(minAmount, gibObjects.Length);
            for (int i = 0; i < gibsAmount; i++)
            {
                gibObjects[i].SetActive(true);
                rotation.x = Random.Range(-360f, 360f);
                rotation.y = Random.Range(-360f, 360f);
                rotation.z = Random.Range(-360f, 360f);
                //go = Instantiate(gibObjects[Random.Range(0,gibObjects.Length)],this.gameObject.transform.position, Quaternion.Euler(rotation));
                go = gibObjects[i];
                go.transform.parent = null;
                if (gibMaterials.Length > 0)
                {
                    rend = go.GetComponent<Renderer>();
                    rend.material = gibMaterials[Random.Range(0,gibMaterials.Length)];
                    rend.material.mainTextureOffset = new Vector2(Random.Range(0f,1f),Random.Range(0f,1f));
                }
                if (randomGibModels.Length > 0)
                {
                    meshFilter = go.GetComponent<MeshFilter>();
                    meshFilter.mesh = randomGibModels[Random.Range(0,randomGibModels.Length)];
                    meshCollider = go.GetComponent<MeshCollider>();
                    meshCollider.sharedMesh = meshFilter.mesh;
                }
                go.transform.position = this.gameObject.transform.position;
                go.transform.localScale = Vector3.one * Random.Range(0.75f, 1.25f);
                gibRB = go.GetComponent<Rigidbody>();
                if (gibRB == null) { Debug.LogError($"Rigid Body not found for {go}!"); continue; }
                force.x = Random.Range(Random.Range(-minMaxForce.y, -minMaxForce.x), Random.Range(minMaxForce.x,minMaxForce.y));
                force.y = Random.Range(minMaxForce.x, minMaxForce.y) * 1.5f;
                force.z = Random.Range(Random.Range(-minMaxForce.y, -minMaxForce.x), Random.Range(minMaxForce.x, minMaxForce.y));
                gibRB.AddForce(force, ForceMode.VelocityChange);
                gibRB.AddTorque(rotation*Random.Range(1,4));
                Destroy(go, giblifetime);
            }
        }
        Destroy(this.gameObject, lifetime);
    }

    
}
