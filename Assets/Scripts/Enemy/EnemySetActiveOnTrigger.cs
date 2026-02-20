using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySetActiveOnTrigger : MonoBehaviour
{
    /// <summary>
    /// add enemies in inspector to be effected by this trigger
    /// </summary>
    public List<Enemy> enemies = new List<Enemy>();

    public GameObject GameObject;
    /// <summary>
    /// If checked, trigger will alert enemies to player's presence. If unchecked, will spawn enmies in 
    /// </summary>
    public bool AlertInstead;

    private void Start()
    {
        //if meant to be alerted, make sure they are active. If meant to be spawned, make sure they are inactive
            foreach (Enemy e in enemies)
            { 
                e.gameObject.SetActive(AlertInstead);
            }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //alerts enemies
            if (AlertInstead)
            {
                foreach (Enemy e in enemies)
                {
                    e.Alert();
                }
            }
            else //otherwise spawns them in
            { 
                foreach (Enemy e in enemies)
                {
                    GameObject o = e.gameObject;
                    o.SetActive(true);
                }
            }
        }
    }

    private void Update()
    {
        CheckValidEnemies();
        if (enemies.Count == 0) GameObject.SetActive(false);
    }

    //this is pretty resource intensive, no?
    /// <summary>
    /// removes dead/destoryed enemies from the list
    /// </summary>
    private void CheckValidEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] == null)
            { 
                enemies.RemoveAt(i);
                i--;
            }
        }
    }

}
