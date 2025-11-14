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
            //yes, i can do e.Alert seperately & omit the first if check, but then at worst case it runs 2 foreach loops instead of 1
            if (AlertInstead)
            {
                foreach (IEnemy e in enemies)
                {
                    e.Alert();
                }
            }
            else 
            { 
                foreach (IEnemy e in enemies)
                {
                    GameObject o = e.gameObject;
                    o.SetActive(true);
                    e.SetStartState(IEnemy.State.chasing);
                    //Debug.Log(e.GetState());
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
