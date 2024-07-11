using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class MeleeAttack : IAttackBehavior
{
    public override void Attack(Soldier attacker, IDamageable target)
    {
        if (target != null && target.Health > 0)
        {
            attacker.StartCoroutine(PerformAttack(attacker, target));
        }
    }

    private IEnumerator PerformAttack(Soldier attacker, IDamageable target)
    {
        // Ensure target is valid before starting attack animation
        if (target != null && target == attacker.CurrentTarget)
        {
            attacker.TriggerAttackAnimation();
            yield return new WaitForSeconds(0.5f);

            // Ensure target is still valid before dealing damage
            if (target != null && target == attacker.CurrentTarget)
            {
                target.TakeDamage(attacker.Stats.Damage);
                if (target.Health <= 0)
                {
                    target.Die();
                }
            }
            yield return new WaitForSeconds(0.5f);
            attacker.ResetAttackAnimation();
        }
    }
}
