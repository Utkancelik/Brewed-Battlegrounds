using UnityEngine;

[System.Serializable]
public class SoldierType
{
    public string soldierName;
    public Sprite soldierIcon;
    public GameObject Prefab; // Reference to the prefab for this soldier type
    public SoldierStats stats;
}