using FMOD.Studio;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Music_Debugger : MonoBehaviour
{
    public Button playMusicButton;
    public Button stopMusicButton;
    public Text playbackStateTxt;

    private EventInstance musicInstance;

    private PLAYBACK_STATE playbackState;
    private PLAYBACK_STATE oldPlaybackState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GetMusicManagerInstance());
    }

    IEnumerator GetMusicManagerInstance()
    {
        yield return new WaitForSeconds(.1f);
        musicInstance = GetMusicInstance();
    }

    EventInstance GetMusicInstance()
    {
        return MusicManager.instance.musicInstance;
    }

    // Update is called once per frame
    void Update()
    {
        musicInstance.getPlaybackState(out playbackState);
        
        UpdateButtons();
        if (playbackState != oldPlaybackState)
            UpdateMusicStateInfo();
    }

    void UpdateButtons()
    {
        playMusicButton.interactable = playbackState.Equals(PLAYBACK_STATE.STOPPED);
        stopMusicButton.interactable = !playbackState.Equals(PLAYBACK_STATE.STOPPED);
    }

    void UpdateMusicStateInfo()
    {
        playbackStateTxt.text = "Music State:";

        switch(playbackState)
        {
            case PLAYBACK_STATE.PLAYING:
                playbackStateTxt.text += " Playing";
                break;
            case PLAYBACK_STATE.SUSTAINING: 
                playbackStateTxt.text += " Sustaining?";
                break;
            case PLAYBACK_STATE.STOPPED:
                playbackStateTxt.text += " Not Playing";
                break;
            case PLAYBACK_STATE.STARTING:
                playbackStateTxt.text += " Starting";
                break;
            case PLAYBACK_STATE.STOPPING:
                playbackStateTxt.text += " Stopping";
                break;
            default:
                break;
        }

        oldPlaybackState = playbackState;
    }

}
