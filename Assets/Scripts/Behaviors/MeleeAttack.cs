using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class MeleeAttack : IAttackBehavior
{
    public override void Attack(Soldier attacker, Soldier target)
    {
        if (target != null && target.Health > 0)
        {
            attacker.StartCoroutine(PerformAttack(attacker, target));
        }
    }

    private IEnumerator PerformAttack(Soldier attacker, Soldier target)
    {
        attacker.TriggerAttackAnimation();
        yield return new WaitForSeconds(0.5f);
        if (target != null && target == attacker.CurrentTarget) // Ensure target is still valid and same
        {
            attacker.DealDamage();
        }
        yield return new WaitForSeconds(0.5f);
        attacker.ResetAttackAnimation();
    }
}
