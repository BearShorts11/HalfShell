using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Another script for Simple Triggers and the likes
public class SimpleSoundEvent : MonoBehaviour
{
    private EventInstance eventInstance;

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
        GUID guid = RuntimeManager.PathToGUID(eventPath);
        if (!guid.IsNull)
            RuntimeManager.PlayOneShot(guid, location);
    }

    public void PlaySound(EventReference audioEvent, Vector3 location)
    { 
        if (!audioEvent.IsNull)
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
        if (!audioEvent.IsNull)
            RuntimeManager.PlayOneShotAttached(audioEvent, attachedObject.gameObject);
    }

    public void PlaySoundAttached(string eventPath, Transform attachedObject)
    {
        FMOD.GUID guid = RuntimeManager.PathToGUID(eventPath);
        if (!guid.IsNull)
            RuntimeManager.PlayOneShotAttached(guid, attachedObject.gameObject);
    }

    // Event Instance - Used for looping fmod events. Parameters may not be completely supported
    private void PlayEventInstance()
    {
        if (!eventInstance.IsUnityNull())
        {
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(this.gameObject));
            RuntimeManager.AttachInstanceToGameObject(eventInstance, this.gameObject);
            eventInstance.start();
        }
    }

    public void StopEventInstance_Fade()
    {
        if (eventInstance.IsUnityNull()) return;
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    public void StopEventInstance()
    {
        if (eventInstance.IsUnityNull()) return;
        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void ReleaseEventInstance()
    {
        if (eventInstance.IsUnityNull()) return;
        eventInstance.release();
    }

    public void ChangeSound(EventReference NewReference)
    {
        if (NewReference.IsNull) return;
        eventInstance.release();
        eventInstance = RuntimeManager.CreateInstance(NewReference);
    }
    public void ChangeSound(string eventPath)
    {
        GUID guid = RuntimeManager.PathToGUID(eventPath);
        if (!guid.IsNull)
        {
            eventInstance.release();
            eventInstance = RuntimeManager.CreateInstance(guid);
        }
    }

    public void PlayInstance(EventReference eventReference)
    {
        ChangeSound(eventReference);
        PlayEventInstance();
    }
    public void PlayInstance(string eventPath)
    {
        ChangeSound(eventPath);
        PlayEventInstance();
    }

    /// <summary>
    /// Modify a parameter of an instance by using command-like arguments
    /// </summary>
    /// <param name="arguments">Arguments to pass separated by semi-colon: "[Parameter Name];[Number Value]"
    /// It is recommended that the FMOD parameter names should not have spaces in them.
    /// </param>
    public void ChangeInstanceParameter(string arguments)
    {
        arguments = arguments.Trim((char)32);
        List<List<string>> argMatrix = new List<List<string>>();
        string[] args = arguments.Split(";");
        

        for (int i = 0; i < args.Length; i++) {
            string[] val = args[i].Split(" ");
            argMatrix.Add(new List<string>());
            for (int j = 0; j < val.Length; j++)
            {
                argMatrix[i].Add(val[j]);
            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            for (int j = 1; j < argMatrix[i].Count; j++) 
            {
                if (int.TryParse(argMatrix[i][j], out var intValue))
                {
                    eventInstance.setParameterByName(argMatrix[i][0], intValue);
                    return;
                }
                else if (float.TryParse(argMatrix[i][j], out float floatValue))
                    eventInstance.setParameterByName(argMatrix[i][0], floatValue);
            }
        }

        for (int i = 0; i < args.Length; i++)
        {
            argMatrix[i].Clear();
            argMatrix[i].TrimExcess();
        }
        argMatrix.Clear();
        argMatrix.TrimExcess();
    }

    private void OnDestroy()
    {
        StopEventInstance();
    }
}
