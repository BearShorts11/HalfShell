using FMODUnity;
using System.Collections;
using UnityEngine;

// Script that activates Random Slow Motion events on enemy death. Could merge into the player behavior script but idk.
public class SlowMo_Manager : MonoBehaviour
{
    public float slowMoChance = 0.05f;
    private PlayerBehavior player;
    private PlayerShooting playerShooting;
    private float slowMoActiveTime = 5;
    private float slowMoTime = 0;
    private float slowMoTimeSaved = 0;
    private bool slowMoActive = false;
    private float currentTimeScale = 1;

    void DramaEvent()
    {
        if (Random.Range(0f,1f) <= slowMoChance)
        {
            StartSlowMo(player.SlowedTime);
        }
    }

    public void StartSlowMo(float setTimeScale)
    {
        if (!slowMoActive)
        {
            slowMoTime = Time.time;
            slowMoActive = true;
            PlayerBehavior.SlowMoActive = true;
            TransitionTime(setTimeScale);
            Invoke(nameof(StopSlowMo), slowMoActiveTime * setTimeScale);
        }
    }

    private void TransitionTime(float setTimeScale = 1f)
    {
        float time = 0, step = 0;
        slowMoTimeSaved = Time.timeScale;
        setTimeScale = Mathf.Clamp01(setTimeScale);

        while (time < 1 && Time.timeScale > setTimeScale)
        {
            step = Time.deltaTime * 0.5f;
            Time.timeScale = Mathf.Lerp(Time.timeScale, setTimeScale, time);
            RuntimeManager.StudioSystem.setParameterByName("Timescale", Time.timeScale);
            currentTimeScale = Time.timeScale;
            //Time.timeScale = slowMoTimeSaved > setTimeScale ? Mathf.Lerp(Time.timeScale, setTimeScale, time) : Mathf.Lerp(Time.timeScale, setTimeScale, 1 - time);
            time += step;
        }

        RuntimeManager.StudioSystem.setParameterByName("Timescale", Time.timeScale);
    }

    public void StopSlowMo()
    {
        if (slowMoActive)
        {
            slowMoActive = false;
            if (!ShellWheelController.shellWheelSelected)
            {
                PlayerBehavior.SlowMoActive = false;
                TransitionTime();
                //Temporary, damn hard coded ShellWheel Timescale.
                RuntimeManager.StudioSystem.setParameterByName("Timescale", 1f);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerBehavior>();
        playerShooting = player.gameObject.GetComponent<PlayerShooting>();
        Enemy.DeathAlert.AddListener(DramaEvent);
    }

    // Update is called once per frame
    void Update()
    {
        //if (PlayerBehavior.SlowMoActive)
        //{
        //    if (!ShellWheelController.shellWheelSelected)
        //        Time.timeScale = currentTimeScale;
        //}
             
        //if (PauseMenu.paused)
        //{
        //    slowMoTimeSaved = Time.time;
        //    if (this.IsInvoking(nameof(StopSlowMo)))
        //    {
        //        this.CancelInvoke(nameof(StopSlowMo));
        //    }
        //}
    }
}
