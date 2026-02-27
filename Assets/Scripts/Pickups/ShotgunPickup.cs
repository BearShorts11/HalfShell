using System.Collections;
using FMODUnity;
using UnityEngine;

public class ShotgunPickup : IPickup
{
    [SerializeField] private EventReference pickupSound;
    [SerializeField] GameObject ApofinalPos;
    [SerializeField] float speed = 0.1f;
    [SerializeField] GameObject shotgunPickup;

    private Vector3 targetPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player = FindFirstObjectByType<PlayerBehavior>();
        targetPosition = ApofinalPos.transform.position;
        StartCoroutine(MoveShotgun());

    }

    // Update is called once per frame
    void Update()
    {
        if (rotate == true)
        {
            Rotate();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            RuntimeManager.PlayOneShot(pickupSound, transform.position);
            Player.EquipShotgun();

            Destroy(gameObject);
        }
    }

    IEnumerator MoveShotgun()
    {
       // while (true)
       // {
           while ((targetPosition - shotgunPickup.transform.position).sqrMagnitude > 0.01f)
           {
                shotgunPickup.transform.position = Vector3.MoveTowards(shotgunPickup.transform.position, targetPosition, speed * Time.deltaTime);
                yield return null;
           }
       // }
    }
}
