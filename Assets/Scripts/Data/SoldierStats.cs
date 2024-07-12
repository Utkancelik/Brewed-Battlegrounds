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

    public SoldierStats(int health, float speed, float attackRange, float detectRange, int damage, int foodCost)
    {
        Health = health;
        Speed = speed;
        AttackRange = attackRange;
        DetectRange = detectRange;
        Damage = damage;
        FoodCost = foodCost;
    }

    public void CopyFrom(SoldierStats other)
    {
        Health = other.Health;
        Speed = other.Speed;
        AttackRange = other.AttackRange;
        DetectRange = other.DetectRange;
        Damage = other.Damage;
        FoodCost = other.FoodCost;
    }
}