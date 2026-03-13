using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;


namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandLeaveDialogue : SequencerCommand
    {
        
           private Dialogue dialogue;
        public void EndConvo()
        {
            dialogue.StartDialogue();
            Stop();
        }
    
    }
}
