using FMODUnity;
using UnityEngine;

public class MeleeEnemy : Enemy, IHasMeleeAttack
{
    [Header("Melee Enemy")]
    public bool PlayerInTrigger { get; set; }

    private void Awake()
    {
        base.Startup();
    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();

        Voice_Update();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player") PlayerInTrigger = true;
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.tag == "Player") PlayerInTrigger = false;
    //}

    public void SetPlayerInTrigger(bool boolean)
    {
        PlayerInTrigger = boolean;
    }

    public override void TakeDamage(float amount)
    {
        if (Health - amount <= 0)
        {
            PlayVoice("event:/Enemy/Cultist/EnemyDeath");
            base.TakeDamage(amount);
            return;
        }

        if (!statusEffected)
        {
            vocalCoolDown = 0.01f;
            if (!IsOnVocalCooldown())
            {
                PlayVoice("event:/Dialogue/cultistsDmg");
            }
        }

        base.TakeDamage(amount);
    }

    public override bool SpottedPlayer()
    {
        if (base.SpottedPlayer()) return true;

        vocalCoolDown = 0.00001f;
        if (!IsOnVocalCooldown())
        {
            PlayVoice("event:/Dialogue/cultistBark");
        }
        return false;
    }

    void Voice_Update()
    {
        if (!IsOnVocalCooldown())
        {

            vocalCoolDown = defaultVocalCoolDown + Random.Range(-1.5f, 1.5f);

            if (statusEffected && statusEffectShell.Type == ShellBase.ShellType.Incindiary)
            {
                vocalCoolDown = 5f;
                PlayVoice("event:/Enemy/EnemyBurning");
                return;
            }

            switch(stateMachine.CurrentState)
            {
                case MeleeAttackState:
                    vocalCoolDown = 0.9f;
                    PlayVoice("event:/Dialogue/cultistsAtk");
                    break;
                //case ChaseState:
                //    PlayVoice("event:/Dialogue/cultistBark");
                //    break;
            }

            //lastVocalization = Time.time;
        }
    }
}
