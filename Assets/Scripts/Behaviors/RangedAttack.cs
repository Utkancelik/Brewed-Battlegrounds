using System.Collections;
using UnityEngine;

public class RangedAttack : IAttackBehavior
{
    public override void Attack(Soldier attacker, Soldier target)
    {
        attacker.StartCoroutine(PerformAttack(attacker, target));
    }

    private IEnumerator PerformAttack(Soldier attacker, Soldier target)
    {
        // Trigger the attack animation
        attacker.TriggerAttackAnimation();
        // Wait for the animation duration
        yield return new WaitForSeconds(1f);
        // Apply damage after the animation
        target.Health -= attacker.GetStats().Damage;
        if (target.Health <= 0)
        {
            GameObject.Destroy(target.gameObject);
            attacker.currentTarget = null; // Clear the target
        }
    }
}