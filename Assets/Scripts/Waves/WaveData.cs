using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWave", menuName = "Waves/Wave")]
public class WaveData : ScriptableObject
{
    public List<GameObject> units;
    public float delayBetweenUnits;
}