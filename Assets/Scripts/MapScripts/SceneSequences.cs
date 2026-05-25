using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Script to handle scripted multi-event sequences (Like multi-trigger relay set up if you're familiar with quake mapping)
// A bit of an advanced trigger, I would say
public class SceneSequences : MonoBehaviour
{
    [Header("Enter Collider Triggers")]
    public List<string> onTriggerEnterTag = new();
    [Header("Exit Collider Triggers")]
    public List<string> onTriggerExitTag = new();

    [Header("Events - Sequence of events \nthat execute in a certain order \nafter a specified amount of time")]
    private int enterEventIndex = 0;
    public List<EventSequence> enterEvents = new List<EventSequence>();
    private int exitEventIndex = 0;
    public List<EventSequence> exitEvents = new List<EventSequence>();

    [Header("Trigger Once - Should the events only trigger once or is this repeatable in the scene")]
    public bool triggerOnce = true;


    private void OnTriggerEnter(Collider other)
    {
        if (enterEvents.Count <= 0) return;
        
        if (onTriggerEnterTag.Count == 0)
        {
            StartEnterEvents();
            return;
        }
        else
        {
            foreach (string tag in onTriggerEnterTag)
                if (other.CompareTag(tag))
                {
                    StartEnterEvents();
                    break;
                }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (exitEvents.Count <= 0) return;

        if (onTriggerExitTag.Count == 0)
        {
            StartExitEvents();
            return;
        }
        else
        {
            foreach (string tag in onTriggerExitTag)
                if (other.CompareTag(tag))
                {
                    StartExitEvents();
                    break;
                }
        }
    }

    // Start[Enter/Exit]Events: When you don't want this component to be a trigger volume object so it gets called by something else, idk
    public void StartEnterEvents()
    {
        Invoke(nameof(RunEnterEvents), enterEvents[enterEventIndex].delay);
    }

    public void StartExitEvents()
    {
        Invoke(nameof(RunExitEvents), exitEvents[exitEventIndex].delay);
    }

    private void RunEnterEvents()
    {
        if (enterEventIndex > enterEvents.Count - 1)
        {
            enterEventIndex = 0;
        }
        
        enterEvents[enterEventIndex].sceneEvent.Invoke();

        enterEventIndex++;

        if (enterEventIndex > enterEvents.Count - 1)
        {
            if (triggerOnce)
            { 
                Destroy(this.gameObject);
                return; // I thought destroying this would already stop the rest of the code from executing below...
            }
            else
                return;
        }

        Invoke(nameof(RunEnterEvents), enterEvents[enterEventIndex].delay);
    }

    private void RunExitEvents()
    {
        if (exitEventIndex > exitEvents.Count - 1)
        {
            exitEventIndex = 0;
        }

        exitEvents[exitEventIndex].sceneEvent.Invoke();

        exitEventIndex++;

        if (exitEventIndex > exitEvents.Count - 1)
        {
            if (triggerOnce)
                Destroy(this.gameObject);
            else
                return;
        }

        Invoke(nameof(RunEnterEvents), exitEvents[exitEventIndex].delay);
    }
}


[System.Serializable]
public class EventSequence
{
    [Tooltip("Wait this long before the Unity executes this event")]
    public float delay = 0f;
    public UnityEvent sceneEvent;
}