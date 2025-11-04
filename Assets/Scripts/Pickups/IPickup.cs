using UnityEngine;

public class IPickup : MonoBehaviour
{
    public int regainAmount;
    public bool infinite;
    public bool rotate;
    public float rotateSpeed = 50f;

    private PlayerBehavior player;
    private PlayerShooting gun;
    private PlayerUI ui;
    public PlayerBehavior Player { get { return player; } set { player = value; } }
    public PlayerShooting Gun { get { return gun; } set { gun = value; } }
    public PlayerUI UI { get { return ui; } set { ui = value; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerBehavior>();
        gun = FindFirstObjectByType<PlayerShooting>();
        ui = FindFirstObjectByType<PlayerUI>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Rotate() => transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);
}
