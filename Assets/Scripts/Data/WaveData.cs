using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewWave", menuName = "Waves/Wave")]
public class WaveData : ScriptableObject
{
    [Serializable]
    public class Group
    {
        public Soldier Soldier;
        public int Amount;
        public float DelayAfterGroup;
    }

    [Serializable]
    public class Wave
    {
        public List<Group> Groups;
    }

    public List<Wave> Waves;
    public float DelayBetweenUnits;
    public float DelayBetweenWaves;
}
