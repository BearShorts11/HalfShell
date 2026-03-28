using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SurvivalWave", menuName = "Scriptable Objects/New Wave")]
public class SurvivalWave : ScriptableObject
{
    public List<SurvivalWave_Squad> Squads;
    public List<GameObject> StrayEnemies;
    public int waveAmount;
    public float spawnRate;
    public int wavetransitionTime;
}
