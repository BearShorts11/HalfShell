using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EnemySetActiveOnTrigger : MonoBehaviour
{
    public List<IEnemy> enemies = new List<IEnemy>();

    public GameObject GameObject;
    public bool AlertInstead;

    private void Start()
    {
            foreach (IEnemy e in enemies)
            { 
                e.gameObject.SetActive(AlertInstead);
            }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {

            if (AlertInstead)
            {
                foreach (IEnemy e in enemies)
                {
                    //set enemey state
                    if (e is RangedEnemy)
                    { }
                    else if (e is MeleeEnemy)
                    { }
                }
            }
            else 
            { 
            
                foreach (IEnemy en in enemies)
                {
                    GameObject o = en.gameObject;
                    o.SetActive(true);
                }
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
