using UnityEngine;

[System.Serializable]
public class SoldierStats
{
    public int Health;
    public float Speed;
    public float AttackRange;
    public float DetectRange;
    public int Damage;
    public int FoodCost;
    public GameObject ArrowPrefab; // Add arrow prefab for ranged units

    public SoldierStats(int health, float speed, float attackRange, float detectRange, int damage, int foodCost, GameObject arrowPrefab)
    {
        Health = health;
        Speed = speed;
        AttackRange = attackRange;
        DetectRange = detectRange;
        Damage = damage;
        FoodCost = foodCost;
        ArrowPrefab = arrowPrefab; // Initialize arrow prefab
    }

    public void CopyFrom(SoldierStats other)
    {
        Health = other.Health;
        Speed = other.Speed;
        AttackRange = other.AttackRange;
        DetectRange = other.DetectRange;
        Damage = other.Damage;
        FoodCost = other.FoodCost;
        ArrowPrefab = other.ArrowPrefab; // Copy arrow prefab
    }
}