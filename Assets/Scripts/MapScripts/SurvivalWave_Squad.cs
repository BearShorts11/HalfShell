using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SurvivalWave_Squad", menuName = "Scriptable Objects/Wave Squad")]
public class SurvivalWave_Squad : ScriptableObject
{
    public List<GameObject> Enemies;
}
