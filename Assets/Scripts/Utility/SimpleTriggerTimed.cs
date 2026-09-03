using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class TimedTriggerEvent
{
    // Activate after this amount of time passes
    [Tooltip("Wait this long before the Unity executes this event")]
    [field: SerializeField] public float time { get; private set; }
    // Set of events to activate
    [SerializeField] public UnityEvent[] triggerEvent = new UnityEvent[new()];
}

public enum TriggerMode
{
    Enter = 0,
    Exit = 1,
    OnKillAll = 2,
    Scripted = 3 // Scripted: You don't need to enter, exit, or kill enemies, you only want the map or something else to interact this externally.
}

/// <summary>
/// Simple Triggers: Timed
/// Only one of the four methods of activation can be used
/// Very similar to SceneSequences, though I think this may be the better one.
/// </summary>
public class SimpleTriggerTimed : MonoBehaviour
{
    public TriggerMode mode = TriggerMode.Enter;
    private float time = 0;
    private float timer = 0;
    private int eventNo = 0;
    public bool triggerOnce = true;
    private bool _triggered = false;
    private bool triggered // This is a field to reset the cancelled bool to false when this gets retriggered
    {
        get {  return _triggered; }
        set
        {
            // cannot set this to true again if it can only trigger once
            if (triggerOnce && _triggered) { _triggered = false; return; }

            if (value == true)
            {
                cancelled = false;
            }
            _triggered = value;
        }
    }
    private bool cancelled = false;

    [Header("Events - Sequence of events \nthat execute in a certain order \nafter a specified amount of time")]
    [Header("Enter Collider Triggers")]
    public List<string> onTriggerEnterTag = new();
    public List<TimedTriggerEvent> onTriggerEnter = new();

    [Header("Exit Collider Triggers")]
    public List<string> onTriggerExitTag = new();
    public List<TimedTriggerEvent> onTriggerExit = new();

    [Header("Kill All of... Triggers")]
    public int InitEnemyCounter;
    public int EnemyDeathsCounter;
    public List<TimedTriggerEvent> onKillAll = new();

    [Header("Scripted Trigger")]
    [Tooltip("List of events to activate when triggered")]
    public List<TimedTriggerEvent> onScripted = new();

    protected List<TimedTriggerEvent> currentMode;

    private void Start()
    {
        // Set the timer for the first event of the selected trigger mode (redundant if the first event's time is set to 0 but better safe than sorry)
        SetTimer();
    }

    private void SetTimer()
    {
        switch (mode)
        {
            case TriggerMode.Enter:
                timer = onTriggerEnter[0].time;
                break;
            case TriggerMode.Exit:
                timer = onTriggerExit[0].time;
                break;
            case TriggerMode.OnKillAll:
                timer = onKillAll[0].time;
                break;
            case TriggerMode.Scripted:
                timer = onScripted[0].time;
                break;
            default:
                break;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (triggered || mode != TriggerMode.Enter || (triggered && triggerOnce)) return;

        if (onTriggerEnter.Count == 1 && onTriggerEnterTag.Count == 0) { triggered = true; return; }

        if (onTriggerEnter.Count != onTriggerEnterTag.Count) { Debug.Log("Trigger Enter tags don't match"); return; }

        for (int i = 0; i < onTriggerEnter.Count; i++)
        {
            if (other.CompareTag(onTriggerEnterTag[i])) { triggered = true; break; }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (triggered || mode != TriggerMode.Exit || (triggered && triggerOnce)) return;

        if (onTriggerExit.Count == 1 && onTriggerExitTag.Count == 0) { triggered = true; return; }

        if (onTriggerExit.Count != onTriggerExitTag.Count) { Debug.Log("Trigger Exit tags don't match"); return; }

        for (int i = 0; i < onTriggerExit.Count; i++)
        {
            if (other.CompareTag(onTriggerExitTag[i])) { triggered = true; break; }
        }
    }

    public void ArenaKillCounter(int value)
    {
        if (triggered && triggerOnce) return;

        EnemyDeathsCounter += value;

        if (!triggered && EnemyDeathsCounter >= InitEnemyCounter && mode == TriggerMode.OnKillAll)
        {
            triggered = true;
            //for (int i = 0; i < onKillAll.Count; i++) { onKillAll[i].Invoke(); }
        }
    }

    public void ActivateScript()
    {
        if (triggered && triggerOnce) return;

        triggered = true;
    }

    public void CancelTrigger()
    {
        if (cancelled & triggerOnce) return;

        cancelled = true;
        triggered = false;
        eventNo = 0;
        time = 0;
        SetTimer();
    }

    private bool CheckEventCount(int Count, int Index)
    {
        return (Index >= Count - 1);
    }

    public void Trigger(List<TimedTriggerEvent> TimedEvents, int i)
    {
        currentMode = TimedEvents;

        if (triggerOnce)
        {
            if (CheckEventCount(TimedEvents.Count, i))
                return;
        }

        // Activate the events in this index
        for (int j = 0; j < TimedEvents[i].triggerEvent.Length; j++)
        {
            TimedEvents[i].triggerEvent[j].Invoke();
        }

        // Set the timer for the next index Event timer
        i++;
        if (i >= TimedEvents.Count) // If this is the final event that was triggered, stop the timer by setting triggered to false
        { 

            triggered = false;
            i = 0; // Reset to the first event if this is a repeatable trigger
            if (!triggerOnce)
                eventNo = 0;
        }
        timer = TimedEvents[i].time;
    }

    public void Update()
    {
        if (!triggered) return;

        time += Time.deltaTime;

        if (time > timer)
        {
            switch (mode)
            {
                case TriggerMode.Enter:
                    Trigger(onTriggerEnter, eventNo);
                    break;
                case TriggerMode.Exit:
                    Trigger(onTriggerExit, eventNo);
                    break;
                case TriggerMode.OnKillAll:
                    Trigger(onKillAll, eventNo);
                    break;
                case TriggerMode.Scripted:
                    Trigger(onScripted, eventNo);
                    break;
                default:
                    break;
            }
            time = 0;
            if (!CheckEventCount(currentMode.Count, eventNo))
                eventNo++;
        }

    }

}
