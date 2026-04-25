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
    static float transitionTime;
    static float time = 0, step = 0;

    [Range(0f,1f)] public float slowMoChance = 0.05f;
    private float slowMoChanceDefault;
    [Range(0f,1f)] public float maxSlowMoChanceBonusFromTime = 0.25f;
    [Tooltip("How long does it take for the bonus chance to reach it's maximum when it's added onto the existing slowMoChance")]
    [Min(0f)] public float timeToReachMaxSlowMoChance = 6000f;
    public float timeSinceLastSlowMo { get; private set; }

    private PlayerBehavior player;
    [SerializeField] private float _SlowMoDurationTime = 2.5f;
    private float slowMoDurationTime
    {
        get { return _SlowMoDurationTime; }
        set { 
            if (value <= 0f)
            {
                _SlowMoDurationTime = slowMoDurationTimeDefault;
            }
            _SlowMoDurationTime = value; 
        }
    }

    float slowMoDurationModified { get { return slowMoDurationTime * setTimeScale; } }

    float slowMoDurationTimeDefault;
    private bool transitioning = false;

    private IEnumerator coroutine;

    public void DramaEvent()
    {
        if (Random.Range(0f,1f) <= (slowMoChance + Mathf.Lerp(0, maxSlowMoChanceBonusFromTime, Time.time - timeSinceLastSlowMo / timeToReachMaxSlowMoChance)))
        {
            StartSlowMo(slowMoScale);
        }
    }

    public void SetSlowMoDuration(float duration)
    {
        slowMoDurationTime = duration;
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
        timeSinceLastSlowMo = Time.time;
        time = 0;
        //Invoke(nameof(StopSlowMo), (slowMoActiveTime * setTimeScale) + (1 * setTimeScale));
    }

    public static void TransitionTimeScale(float timeScale = 1f)
    {
        transitionTime = 0; step = 0;
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
            slowMoDurationTime = slowMoDurationTimeDefault;
            slowMoChance = slowMoChanceDefault;
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

        slowMoDurationTimeDefault = _SlowMoDurationTime;
        slowMoChanceDefault= slowMoChance;
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

        if (!PauseMenu.paused && (time < slowMoDurationModified || transitionTime < 1))
        {
            step = Time.deltaTime;
            time += step;
            transitionTime += step;

            Time.timeScale = Mathf.Lerp(Time.timeScale, setTimeScale, Mathf.Clamp01(transitionTime));

            if (transitioning)
            {
                RuntimeManager.StudioSystem.setParameterByName("Timescale", Time.timeScale);
                if (Time.timeScale == 1 && time >= 1 && transitioning)
                    transitioning = false;
            }
            if ((time >= slowMoDurationModified) && slowMoActive)
            {
                StopSlowMo();
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
