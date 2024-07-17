using UnityEngine;

[System.Serializable]
public class SoldierType
{
    public string SoldierName;
    public Sprite SoldierIcon;
    public GameObject Prefab;
    public SoldierStats Stats;
    public bool IsUnlocked;
    public int UnlockCost;

    public SoldierType(string name, Sprite icon, GameObject prefab, SoldierStats stats, int unlockCost)
    {
        SoldierName = name;
        SoldierIcon = icon;
        Prefab = prefab;
        Stats = stats;
        UnlockCost = unlockCost;
        IsUnlocked = PlayerPrefs.GetInt(name, 0) == 1; // Load unlock status
    }

    public void Save()
    {
        PlayerPrefs.SetInt(SoldierName, IsUnlocked ? 1 : 0);
    }
}


