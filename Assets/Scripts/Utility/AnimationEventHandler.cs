using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class AnimationEvent
{
    [field:SerializeField] public string EventName { get; private set; }
    [SerializeField] public UnityEvent[] events = new UnityEvent[new()];
}

public class AnimationEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] bodyGroup;
    [SerializeField] private Transform[] transformPoints;
    [SerializeField] private AnimationEvent[] animationEvents;
    Dictionary<string, AnimationEvent> animationDictionary = new();

    private void Start()
    {
        foreach (AnimationEvent _event in animationEvents)
        {
            animationDictionary.Add(_event.EventName, _event);
        }
    }

    public void PlayEvent(string EventName)
    {
        //bool isTrue = animationDictionary.ContainsKey(EventName);
        if (animationDictionary.ContainsKey(EventName))
        {
            for (int i = 0; i < animationDictionary[EventName].events.Length; i++) {
                animationDictionary[EventName].events[i].Invoke();
            }
        }
    }

    //if this isn't used in the next few weeks can I get rid of this? -N
    // You may. -V
    public void PlaySound(string path)
    {
        RuntimeManager.PlayOneShotAttached(path, this.gameObject);
    }
    
    // will these work in the animation event once we have modular shells set up? -V
    public void HideBodyGroup(int index)
    {
        if (bodyGroup.Length > 0)
            bodyGroup[Mathf.Clamp(index, 0, bodyGroup.Length-1)].SetActive(false);
    }
    public void ShowBodyGroup(int index)
    {
        if (bodyGroup.Length > 0)
            bodyGroup[Mathf.Clamp(index, 0, bodyGroup.Length-1)].SetActive(true);
    }
}
