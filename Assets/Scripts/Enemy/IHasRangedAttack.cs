using System;
using UnityEngine;

public interface IHasRangedAttack
{
    public void Shoot();

    public float ShotOffset { get; }
}