using Assets.Scripts;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(SimpleTriggerTimed))]
public class SimpleDamageTrigger : MonoBehaviour, IDamageable
{
    public float Health { get; set; }

    public float maxHealth { get; private set; }

    public bool bSpecificShell = false;

    public float reactivationCooldown = 0.5f;

    private float coolDownTime = 0;

    [field: SerializeField] public ShellBase.ShellType shellType;

    [SerializeField] SimpleTriggerTimed trigger;

    public void TakeDamage(float amount, ShellBase.ShellType type)
    {
        if (bSpecificShell)
        {
            if (shellType != type)
                return;
        }
        TakeDamage(amount);
    }
    public void TakeDamage(float amount)
    {
        if (Time.time > coolDownTime)
        {
            Trigger();
            coolDownTime = Time.time + reactivationCooldown;
        }
    }

    void Trigger()
    {
        if (trigger)
        {
            trigger.ActivateScript();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trigger = GetComponent<SimpleTriggerTimed>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
