using UnityEngine;

[System.Serializable]
public class SoldierType
{
    public string SoldierName;
    public Sprite SoldierIcon;
    public GameObject Prefab;
    public bool IsUnlocked;
    public int UnlockCost;
    public int Era;
    public int FoodCost;

    public SoldierType(string name, Sprite icon, GameObject prefab, int unlockCost, int era, int foodCost)
    {
        SoldierName = name;
        SoldierIcon = icon;
        Prefab = prefab;
        UnlockCost = unlockCost;
        Era = era;
        FoodCost = foodCost;
        IsUnlocked = PlayerPrefs.GetInt(name, 0) == 1; // Load unlock status
    }

    public void Save()
    {
        PlayerPrefs.SetInt(SoldierName, IsUnlocked ? 1 : 0);
    }
}


