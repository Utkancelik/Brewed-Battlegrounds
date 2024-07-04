using System.Collections;
using UnityEngine;

public class MeleeAttack : IAttackBehavior
{
    public void Attack(Soldier attacker, Soldier target)
    {
        if (target != null && target.Health > 0)
        {
            attacker.StartCoroutine(PerformAttack(attacker, target));
        }
    }

    private IEnumerator PerformAttack(Soldier attacker, Soldier target)
    {
        // Trigger the attack animation
        attacker.TriggerAttackAnimation();
        // Wait for the critical point in the animation to deal damage
        yield return new WaitForSeconds(0.5f);
        attacker.DealDamage();
        // Wait for the animation to end
        yield return new WaitForSeconds(0.5f);
        attacker.ResetAttackAnimation(); // Reset the attack animation trigger
    }
}