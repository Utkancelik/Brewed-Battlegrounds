using UnityEngine;

[RequireComponent(typeof(MeleeAttack), typeof(NormalMove))]
public class EnemyInfantry : Soldier
{
    private void Awake()
    {
        Health = 100;
        stats = GameManager.Instance.EnemyInfantryStats; // Access stats from GameManager
        base.Awake(); // Ensure base class Awake is called
    }

    public override void Display()
    {
        // Display Enemy Infantry-specific visuals
    }
}

// Similar classes for PlayerArcher, EnemyArcher, PlayerCavalry, EnemyCavalry