using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using FMODUnity;
using System.Diagnostics.CodeAnalysis;

using System;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;


public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI txtComp;
    private string[] lines = 
        {
        /*0*/"Light these fiends up", //Start of First Strike Lines (Shooting the door)
        /*1*/"Are you waiting for them to shoot first? Fire!",
        /*2*/"There are raiders everywhere. We must take them out!", //Start of Mission 1 lines
        /*3*/"Defend! Defend! Defend!",
        /*4*/"We must escape!",
        /*5*/"No use waiting on them to come out.", //Start of burial lines
        /*6*/"I was the exception Kerth. You don't need anybody else."
        };
    public float txtSpeed;

    private int index;
    private bool speech;

    private static System.Random rand = new System.Random();
    private static int GetRandomNumber(int max) => rand.Next(max);
    private static int GetRandomNumber(int min, int max) => rand.Next(min, max);
    private PlayerShooting playerShoot;
    public GameObject Sbutton;
    public GunFace gunFace;
    private PlayerBehavior player;

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
       //Clear any text on game start
        txtComp.text = string.Empty;
        Debug.Log("Dialogue text cleared.");
        Sbutton.SetActive(false);
        //StartDialogue();
        player = FindFirstObjectByType<PlayerBehavior>();
        playerShoot = FindFirstObjectByType<PlayerShooting>();
    }

     void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            if (playerShoot.lookingAtGun == false)
            {

                Debug.Log("player is looking at gun.");
                Sbutton.SetActive(true);
                PlayerBehavior.UnlockCursor();
            }
            if (playerShoot.lookingAtGun == true)
            {
                gunFace.StopTalking();
                Sbutton.SetActive(false);
                PlayerBehavior.LockCursor();
                txtComp.text = string.Empty;
                
            }
        }
    }

    public void Click()
    {
        txtComp.text = string.Empty ;
        StartDialogue();
    }

    void StartDialogue()
    {
      
        gunFace.Talk();
        RandomMissionOneLine();
        StartCoroutine(TypeLine());
        Debug.Log("Line End hit");
        
        
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

    void RandomMissionOneLine()
    {

        index = GetRandomNumber(2, 4);
        Debug.Log("Random line chosen.");
    }
}
