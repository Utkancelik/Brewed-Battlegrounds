using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class MeleeAttack : IAttackBehavior
{
    private bool isAttacking; // Flag to indicate if the melee soldier is currently attacking

    public override void Attack(Soldier attacker, IDamageable target)
    {
        if (target != null && target.Health > 0 && !isAttacking)
        {
            attacker.StartCoroutine(PerformAttack(attacker, target));
        }
    }

    private IEnumerator PerformAttack(Soldier attacker, IDamageable target)
    {
        isAttacking = true; // Set the flag to true when the attack starts
        if (target != null && target == attacker.CurrentTarget)
        {
            attacker.TriggerAttackAnimation();
            yield return new WaitForSeconds(0.5f);

            if (target != null && target == attacker.CurrentTarget)
            {
                target.TakeDamage(attacker.Stats.Damage);
            }
            yield return new WaitForSeconds(0.5f);
            attacker.ResetAttackAnimation();
        }
        isAttacking = false; // Reset the flag after the attack finishes
    }
}



