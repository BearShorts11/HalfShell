using FMOD.Studio;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Music_Debugger : MonoBehaviour
{
    public Button playMusicButton;
    public Button stopMusicButton;
    //public Dropdown musicStageID;

    private EventInstance musicInstance;

    private PLAYBACK_STATE playbackState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GetMusicManagerInstance());
    }

    IEnumerator GetMusicManagerInstance()
    {
        yield return new WaitForSeconds(1f);
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
        playMusicButton.interactable = playbackState.Equals(PLAYBACK_STATE.STOPPED);
        stopMusicButton.interactable = !playbackState.Equals(PLAYBACK_STATE.STOPPED);
    }

    //private void UpdateMusicStage()
    //{
    //    if (MusicManager.instance != null && !MusicManager.instance.musicInstance.IsUnityNull())
    //        MusicManager.instance.SetMusicStage(musicStageID.value);
    //}
}
