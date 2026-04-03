using FMODUnity;
using UnityEngine;

// Another script for Simple Triggers and the likes
public class SimpleSoundEvent : MonoBehaviour
{
    public void PlaySound(string eventName)
    {
        PlaySound(EventReference.Find(eventName));
    }

    public void PlaySound(EventReference audioEvent)
    {
        PlaySound(audioEvent, this.gameObject.transform.position);
    }

    public void PlaySoundAttached(EventReference audioEvent, Transform attachedObject)
    {
        RuntimeManager.PlayOneShotAttached(audioEvent, attachedObject.gameObject);
    }

    public void PlaySound(EventReference audioEvent, Vector3 location)
    { 
        RuntimeManager.PlayOneShot(audioEvent, location);
    }
}
