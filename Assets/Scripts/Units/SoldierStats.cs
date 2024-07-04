public class SoldierStats
{
    public int Damage { get; set; }
    public float Speed { get; set; }
    public float DetectRange { get; set; }
    public float AttackRange { get; set; }

    public SoldierStats(int damage, float speed, float detectRange, float attackRange)
    {
        Damage = damage;
        Speed = speed;
        DetectRange = detectRange;
        AttackRange = attackRange;
    }

    public void UpgradeDamage(int amount)
    {
        Damage += amount;
    }

    public void UpgradeSpeed(float amount)
    {
        Speed += amount;
    }

    public void UpgradeDetectRange(float amount)
    {
        DetectRange += amount;
    }

    public void UpgradeAttackRange(float amount)
    {
        AttackRange += amount;
    }
}