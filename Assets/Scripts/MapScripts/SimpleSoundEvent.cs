using FMODUnity;
using UnityEngine;

// Another script for Simple Triggers and the likes
public class SimpleSoundEvent : MonoBehaviour
{
    public void PlaySound(string eventPath)
    {
        PlaySound(eventPath, this.gameObject.transform.position);
    }

    public void PlaySound(EventReference audioEvent)
    {
        PlaySound(audioEvent, this.gameObject.transform.position);
    }

    public void PlaySound(string eventPath, Vector3 location)
    {
        RuntimeManager.PlayOneShot(eventPath, location);
    }

    public void PlaySound(EventReference audioEvent, Vector3 location)
    { 
        RuntimeManager.PlayOneShot(audioEvent, location);
    }

    public void PlaySoundAttached(string eventPath)
    {
        PlaySoundAttached(eventPath, this.gameObject.transform);
    }

    public void PlaySoundAttached(EventReference audioEvent)
    {
        PlaySoundAttached(audioEvent, this.gameObject.transform);
    }

    public void PlaySoundAttached(EventReference audioEvent, Transform attachedObject)
    {
        RuntimeManager.PlayOneShotAttached(audioEvent, attachedObject.gameObject);
    }

    public void PlaySoundAttached(string eventPath, Transform attachedObject)
    {
        RuntimeManager.PlayOneShotAttached(eventPath, attachedObject.gameObject);
    }
}
