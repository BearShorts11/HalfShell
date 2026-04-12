using FMODUnity;
using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

// Script that activates Random Slow Motion events on enemy death. Tied to the Player Prefab
public class SlowMo_Manager : MonoBehaviour
{
    [field: SerializeField] public static float setTimeScale { get; private set; } = 1.0f;
    [field: SerializeField] public static float slowMoScale { get; private set; } = 0.25f;
    [field: SerializeField] public static bool slowMoActive { get; private set; } = false;
    static float time = 0, step = 0;

    [Range(0f,1f)] public float slowMoChance = 0.05f;
    private PlayerBehavior player;
    private float slowMoActiveTime = 5;
    private bool transitioning = false;

    public void DramaEvent()
    {
        if (Random.Range(0f,1f) <= slowMoChance)
        {
            StartSlowMo(slowMoScale);
        }
    }

    public void StartSlowMo(float setTimeScale)
    {
        if (!slowMoActive)
        {
            slowMoActive = true;
        }
        else
        {
            CancelInvoke(nameof(StopSlowMo));
        }
        transitioning = true;
        TransitionTimeScale(setTimeScale);
        Invoke(nameof(StopSlowMo), slowMoActiveTime * setTimeScale);
    }

    public static void TransitionTimeScale(float timeScale = 1f)
    {
        time = 0; step = 0;
        timeScale = Mathf.Clamp01(timeScale);
        setTimeScale = timeScale;
    }

    public void StopSlowMo()
    {
        if (slowMoActive)
        {
            slowMoActive = false;
            transitioning = true;
            if (ShellWheelController.shellWheelSelected && setTimeScale > player.SlowedTime)
            {
                TransitionTimeScale(player.SlowedTime);
            }
            else
            {
                TransitionTimeScale();
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerBehavior>();
        if (player == null)
            player = FindFirstObjectByType<PlayerBehavior>();
        //Enemy.DeathAlert.AddListener(DramaEvent);
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

        if (!PauseMenu.paused && time < 1 && Time.timeScale != setTimeScale)
        {
            step = Time.deltaTime;
            time += step;
            time = Mathf.Clamp01(time);
            Time.timeScale = Mathf.Lerp(Time.timeScale, setTimeScale, time);
            if (transitioning)
                RuntimeManager.StudioSystem.setParameterByName("Timescale", Time.timeScale);

            if (time >= 1)
                transitioning = false;
            //Time.timeScale = slowMoTimeSaved > setTimeScale ? Mathf.Lerp(Time.timeScale, setTimeScale, time) : Mathf.Lerp(Time.timeScale, setTimeScale, 1 - time);
        }
    }
}
