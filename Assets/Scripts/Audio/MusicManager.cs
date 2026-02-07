using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance { get; private set; }

    public EventReference musicToPlay;

    public EventInstance musicInstance { get; private set;}

    private void Awake()
    {
        if (instance != null)
            Debug.LogError("More than one Music Manager is found in the scene! Please keep only one at a time.");
        instance = this;
    }

    private void SetupMusic()
    {
        if (musicToPlay.IsNull) {
            Debug.LogError("Error! Music Manager in the scene but no Music Event is defined!");
            return; 
        }
        musicInstance = RuntimeManager.CreateInstance(musicToPlay);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetupMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayMusic()
    {
        musicInstance.start();
    }

    private void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void SetMusicStage(int StageID)
    {
        musicInstance.setParameterByName("Music_Stage", StageID);
    }

    public void PlayStopMusic(bool bPlay)
    {
        if (musicInstance.IsUnityNull()) return;

        PLAYBACK_STATE playbackState;
        musicInstance.getPlaybackState(out playbackState);
        if (!playbackState.Equals(PLAYBACK_STATE.STOPPED) && !bPlay)
            StopMusic();
        else if (playbackState.Equals(PLAYBACK_STATE.STOPPED) && bPlay)
            PlayMusic();
    }
}
