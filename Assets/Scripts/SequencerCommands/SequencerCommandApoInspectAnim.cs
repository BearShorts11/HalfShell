using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandApoInspectAnim : SequencerCommand
    {
        private PlayerShooting shoot;
        public void InspectAnim()
        {
            shoot.GunInspect();
            Debug.Log("Play Anim?");
            Stop();
        }
    }
}
