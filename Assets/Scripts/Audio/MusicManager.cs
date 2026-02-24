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
    private void PlayMusic()
    {
        musicInstance.start();
    }

    private void StopMusic()
    {
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    // In theory, changes to a new music. Currently Untested.
    public void ChangeMusic(EventReference NewMusic)
    {
        StopMusic();
        musicInstance.release();
        musicToPlay = NewMusic;
        musicInstance = RuntimeManager.CreateInstance(musicToPlay);
    }

    /// <summary>
    /// Changes a section of the music to a corresponding section based on specific phase specified via Number ID
    /// (Ranges from 0 - 3, 3 being unused for now)
    /// 0 - Calm? Not sure yet.
    /// 1 - Combat
    /// 2 - End (May stop the music from playing until ID is set to any number other than 2 and 3, then PlayStopMusic is called to play the music again.)
    /// </summary>
    /// <param name="PhaseID"></param>
    public void SetMusicPhase(int PhaseID)
    {
        musicInstance.setParameterByName("Music_Stage", PhaseID);
    }

    /// <summary>
    /// Starts or stops music playback based on the specified flag.
    /// </summary>
    /// <param name="bPlay">If true, starts music playback; if false, stops music playback.</param>
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
