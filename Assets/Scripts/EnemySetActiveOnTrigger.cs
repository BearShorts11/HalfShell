using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySetActiveOnTrigger : MonoBehaviour
{
    public List<IEnemy> enemies = new List<IEnemy>();

    public GameObject GameObject;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            foreach (IEnemy en in enemies)
            {
                GameObject o = en.gameObject;
                o.SetActive(true);
            }
        }
    }

    private void Update()
    {
        CheckEnemies();
        if (enemies.Count == 0) GameObject.SetActive(false);
    }

    //this is pretty resource intensive, no?
    private void CheckEnemies()
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
