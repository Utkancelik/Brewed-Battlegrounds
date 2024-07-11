using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class RangedAttack : IAttackBehavior
{
    public override void Attack(Soldier attacker, IDamageable target)
    {
        attacker.StartCoroutine(PerformAttack(attacker, target));
    }

    private IEnumerator PerformAttack(Soldier attacker, IDamageable target)
    {
        attacker.TriggerAttackAnimation();
        yield return new WaitForSeconds(1f);
        if (target != null && target == attacker.CurrentTarget) // Ensure target is still valid and same
        {
            target.TakeDamage(attacker.Stats.Damage); // Handle damage directly here
            if (target.Health <= 0)
            {
                target.Die();
                attacker.CurrentTarget = null;
            }
        }
        attacker.ResetAttackAnimation();
    }
}




