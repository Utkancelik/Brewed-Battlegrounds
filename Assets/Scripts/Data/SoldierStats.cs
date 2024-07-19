using UnityEngine;

[System.Serializable]
public class SoldierStats
{
    public int Health;
    public float Speed;
    public float AttackRange;
    public float DetectRange;
    public int Damage;
    public GameObject ArrowPrefab;

    public SoldierStats(int health, float speed, float attackRange, float detectRange, int damage, GameObject arrowPrefab)
    {
        Health = health;
        Speed = speed;
        AttackRange = attackRange;
        DetectRange = detectRange;
        Damage = damage;
        ArrowPrefab = arrowPrefab;
    }
}

