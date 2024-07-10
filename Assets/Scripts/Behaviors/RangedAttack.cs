using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class RangedAttack : IAttackBehavior
{
    public override void Attack(Soldier attacker, Soldier target)
    {
        attacker.StartCoroutine(PerformAttack(attacker, target));
    }

    private IEnumerator PerformAttack(Soldier attacker, Soldier target)
    {
        attacker.TriggerAttackAnimation();
        yield return new WaitForSeconds(1f);
        target.TakeDamage(attacker.Stats.Damage);
        if (target.Health <= 0)
        {
            GameObject.Destroy(target.gameObject);
            attacker.CurrentTarget = null;
        }
    }
}

