using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class SurvivalWave_Events
{
    [Tooltip("Wave Index - What wave these events should trigger in")]
    public int waveIndex = 0;
    [Tooltip("Start Events - Activates when the wave starts")]
    public List<UnityEvent> startEvents;
    [Tooltip("Clear Events - Activates when the wave ends")]
    public List<UnityEvent> clearEvents;

    public void RunStartEvents()
    {
        for (int i = 0; i < startEvents.Count; i++)
        {
            startEvents[i].Invoke();
        }
    }

    public void RunClearEvents()
    {
        for (int i = 0; i < clearEvents.Count; i++)
        {
            clearEvents[i].Invoke();
        }
    }
}
