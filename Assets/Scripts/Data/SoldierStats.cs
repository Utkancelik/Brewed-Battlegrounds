using UnityEngine;

[System.Serializable]
public class SoldierStats
{
    [SerializeField] private int damage;
    [SerializeField] private float speed;
    [SerializeField] private float detectRange;
    [SerializeField] private float attackRange;

    public int Damage { get => damage; set => damage = value; }
    public float Speed { get => speed; set => speed = value; }
    public float DetectRange { get => detectRange; set => detectRange = value; }
    public float AttackRange { get => attackRange; set => attackRange = value; }

    public SoldierStats(int damage, float speed, float detectRange, float attackRange)
    {
        this.damage = damage;
        this.speed = speed;
        this.detectRange = detectRange;
        this.attackRange = attackRange;
    }

    public void UpgradeDamage(int amount)
    {
        damage += amount;
    }

    public void UpgradeSpeed(float amount)
    {
        speed += amount;
    }

    public void UpgradeDetectRange(float amount)
    {
        detectRange += amount;
    }

    public void UpgradeAttackRange(float amount)
    {
        attackRange += amount;
    }
}