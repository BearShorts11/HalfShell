using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;
using FMODUnity;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandFMODAudioEvent : SequencerCommand
    {
        public void Awake()
        {
            string FMODEvent = GetParameter(0);

            if (!string.IsNullOrEmpty(FMODEvent))
            {
                RuntimeManager.PlayOneShot(FMODEvent);
                Stop();
            }
        }
    }
}
