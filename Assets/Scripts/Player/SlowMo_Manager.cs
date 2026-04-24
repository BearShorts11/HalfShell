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
    [SerializeField] private float slowMoActiveTime = 2.5f;
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
            if (IsInvoking(nameof(StopSlowMo)))
                CancelInvoke(nameof(StopSlowMo));
        }
        RuntimeManager.PlayOneShot("event:/UI/SlowMo_Activate");
        transitioning = true;
        TransitionTimeScale(setTimeScale);
        Invoke(nameof(StopSlowMo), (slowMoActiveTime * setTimeScale) + (1 * setTimeScale));
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
            if (ShellWheelController.shellWheelSelected && setTimeScale > player.SlowedTime)
            {
                TransitionTimeScale(player.SlowedTime);
            }
            else
            {
                TransitionTimeScale();
            }
            RuntimeManager.PlayOneShot("event:/UI/SlowMo_Deactivate");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerBehavior>();
        if (player == null)
            player = FindFirstObjectByType<PlayerBehavior>();
        
        if (Time.timeScale != 1 || setTimeScale != 1)
        {
            Time.timeScale = 1;
            setTimeScale = 1;
        }
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

        if (!PauseMenu.paused && time < 1)
        {
            step = Time.deltaTime;
            time += step;
            if (time > 1)
                time = Mathf.Clamp01(time);
            
            Time.timeScale = Mathf.Lerp(Time.timeScale, setTimeScale, time);

            if (transitioning)
            {
                RuntimeManager.StudioSystem.setParameterByName("Timescale", Time.timeScale);
                if (Time.timeScale == 1 && time >= 1 && transitioning)
                    transitioning = false;
            }
        }
    }

    private void OnDestroy()
    {
        if (setTimeScale != 1 || Time.timeScale != 1)
        {
            Time.timeScale = 1;
            setTimeScale = 1;
        }
    }
}
