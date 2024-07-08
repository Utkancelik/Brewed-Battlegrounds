public class FutureSniper : Soldier
{
    private void Awake()
    {
        Health = 100;
        stats = GameManager.Instance.PlayerInfantryStats;
        base.Awake();
    }

    public override void Display()
    {
        // Display Future Sniper-specific visuals
    }
}
