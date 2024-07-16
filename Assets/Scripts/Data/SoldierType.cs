using UnityEngine;

[System.Serializable]
public class SoldierType
{
    public string soldierName;
    public Sprite soldierIcon;
    public GameObject Prefab; // Reference to the prefab for this soldier type
    public SoldierStats stats;
    public bool isUnlocked;
    public int unlockCost;

    public SoldierType(string name, Sprite icon, GameObject prefab, SoldierStats stats, int unlockCost)
    {
        soldierName = name;
        soldierIcon = icon;
        Prefab = prefab;
        this.stats = stats;
        this.unlockCost = unlockCost;
        isUnlocked = false; // Default to locked
    }
}
