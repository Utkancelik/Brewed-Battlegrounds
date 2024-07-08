using UnityEngine;

[RequireComponent(typeof(MeleeAttack), typeof(NormalMove))]
public class PlayerInfantry : Soldier
{
    private void Awake()
    {
        Health = 100;
        stats = GameManager.Instance.PlayerInfantryStats;
        base.Awake();
    }

    public override void Display()
    {
        // Display Player Infantry-specific visuals
    }
}