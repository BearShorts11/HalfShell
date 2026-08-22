using System;
using UnityEngine;

/// <summary>
/// Mannequin Idle animations made into an enum list. To only be used with the mannequin enemy.
/// </summary>
public enum MannequinIdle
{
    Idle1,
    Idle2,
    Idle3,
    Idle4,
    Idle5,
    Idle6,
    Idle7,
    Idle8
}

public class MannequinPoses : MonoBehaviour
{

    public MannequinIdle idleAnim;
    public string[] idleAnims { get; private set; } = new string[8];
    [SerializeField] private Animator animator;
    [SerializeField] private LookScript look;

    private MannequinEnemy enemy;

    void Awake()
    {
        animator = TryGetComponent<Animator>(out Animator component) ? component : GetComponentInChildren<Animator>();            
    }

    void Start()
    {
        SetupIdle();
    }

    public void SetupIdle()
    {
        idleAnims = Enum.GetNames(typeof(MannequinIdle));
        if (animator != null)
        {
            PlayIdleAnim(idleAnim);
        }
    }

    public void PlayIdleAnim(MannequinIdle idle)
    {
        if (animator != null)
        {
            animator.CrossFade(idleAnims[(int)idle], 0.1f);
            if (look)
            {
                if (IsInvoking(nameof(UpdateHead)))
                    CancelInvoke(nameof(UpdateHead));
                Invoke(nameof(UpdateHead), 0.1f);
            }
        }
    }

    private void UpdateHead()
    {
        look.UpdateHead();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}