public class PlayerArcher : Soldier
{
    private void Awake()
    {
        Health = 100;
        stats = GameManager.Instance.PlayerInfantryStats;
        base.Awake();
        SetAttackBehavior(new RangedAttack());
    }

    public override void Display()
    {
        // Display Player Archer-specific visuals
    }
}
