using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class SimpleTrigger : MonoBehaviour
{
    [Header("Enter Collider Triggers")]
    public List<string> onTriggerEnterTag;
    public List<UnityEvent> onTriggerEnter;

    [Header("Exit Collider Triggers")]
    public List<string> onTriggerExitTag;
    public List<UnityEvent> onTriggerExit;

    [Header("Kill All of... Triggers")]
    public int InitEnemyCounter;
    public int EnemyDeathsCounter;
    public List<UnityEvent> onKillAll;

    public void OnTriggerEnter(Collider other)
    {
        if (onTriggerEnter.Count == 1 && onTriggerEnterTag.Count == 0) { onTriggerEnter[0].Invoke(); return; }

        if (onTriggerEnter.Count != onTriggerEnterTag.Count) {  Debug.Log("Trigger Enter tags don't match"); return; }

        for (int i = 0; i < onTriggerEnter.Count; i++)
        {
            if (other.CompareTag(onTriggerEnterTag[i])) { onTriggerEnter[i].Invoke(); }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (onTriggerExit.Count == 1 && onTriggerExitTag.Count == 0) { onTriggerExit[0].Invoke(); return; }

        if (onTriggerExit.Count != onTriggerExitTag.Count) { Debug.Log("Trigger Exit tags don't match"); return; }

        for (int i = 0; i < onTriggerExit.Count; i++)
        {
            if (other.CompareTag(onTriggerExitTag[i])) { onTriggerExit[i].Invoke(); }
        }
    }

    public void ArenaKillCounter(int value)
    {
        EnemyDeathsCounter += value;

        if (EnemyDeathsCounter >= InitEnemyCounter)
        {
            for (int i = 0; i < onKillAll.Count; i++) { onKillAll[i].Invoke(); }
        }
    }
}
