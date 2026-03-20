using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;


namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandLeaveDialogue : SequencerCommand
    {
        
           private PlayerShooting shoot;
        public void EndConvo()
        {
            shoot.ForceLookAtGun();
            Stop();
        }
    
    }
}
