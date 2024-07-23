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
    
    public void Save()
    {
        PlayerPrefs.SetInt(SoldierName, IsUnlocked ? 1 : 0);
    }
}


