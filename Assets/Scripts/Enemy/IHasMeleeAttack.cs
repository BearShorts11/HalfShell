using System;
using UnityEngine;

public interface IHasMeleeAttack
{
    public bool moveWhileAttacking { get; set; }
    public bool PlayerInTrigger { get; set; }

    public void SetPlayerInTrigger(bool boolean);
}