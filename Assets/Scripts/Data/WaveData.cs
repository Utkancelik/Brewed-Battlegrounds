using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWave", menuName = "Waves/Wave")]
public class WaveData : ScriptableObject
{
    [Serializable]
    public class WaveDataEntry
    {
        public Soldier Soldier;
        public int Amount;
    }
    
    public List<WaveDataEntry> Waves;
    public float delayBetweenUnits;
    public float delayBetweenWaves;
}