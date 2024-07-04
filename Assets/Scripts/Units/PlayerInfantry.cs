public class PlayerInfantry : Soldier
{
    private void Awake()
    {
        Health = 100;
        stats = GameManager.Instance.PlayerInfantryStats; // Access stats from GameManager
        base.Awake(); // Ensure base class Awake is called
    }

    public override void Display()
    {
        // Display Player Infantry-specific visuals
    }
}