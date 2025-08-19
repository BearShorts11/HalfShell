using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]
public class EnemyBehavior : MonoBehaviour
{
    public float walkSpeed = 12f;
    public float gravity = 20f;
    public int health;
    public int maxHealth = 100;
    public float detectionRadius = 10;

    private GameObject enemyObject;

    public GameObject playerObject;
    private PlayerBehavior player;
    bool alert = false;


    // Initializes Enemy upon Start, giving them max health and grabbing the Player Object
    void Start()
    {
        enemyObject = this.gameObject;

        playerObject = GameObject.Find("Player");
        player = playerObject.GetComponent<PlayerBehavior>();

        health = maxHealth;
    }


    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Detetcts if the player is within detetction radius.
        // If so, persues. Enemies do not stop persuing the player post detetction
        if (distanceToPlayer <= detectionRadius)
        {
            alert = true;
        }

        if (alert)
        {
            // Rotates to "look" at the player
            Vector3 direction = (player.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
        }
    }
}
