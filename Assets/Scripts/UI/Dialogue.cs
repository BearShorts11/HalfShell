using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using System.Diagnostics.CodeAnalysis;
using UnityEngine.Windows;
using System;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI txtComp;
    public string[] lines;
    public float txtSpeed;

    private int index;

    private static System.Random rand = new System.Random();
    private static int GetRandomNumber(int max) => rand.Next(max);
    private static int GetRandomNumber(int min, int max) => rand.Next(min, max);

    public EventReference dialogue;
    /// <summary>
    /// Plays a sound from the game object that this script is attached to, in this case, the player
    /// </summary>
    /// <param name="eventReference"> The path to the FMOD sound event </param>
    private void PlaySound(EventReference eventReference)
    {
        RuntimeManager.PlayOneShotAttached(eventReference, this.gameObject);
    }
    void Start()
    {
       
        txtComp.text = string.Empty;
        StartDialogue();
    }

     void Update()
    {
        
    }
    void StartDialogue()
    {
        RandomLine();
        StartCoroutine(TypeLine());
    }

    

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            PlaySound(dialogue);
            txtComp.text += c;
            yield return new WaitForSeconds(txtSpeed);
        }
    }

    void RandomLine()
    {

        index = GetRandomNumber(0, 2);
        
    }
}
