using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class RangedAttack : IAttackBehavior
{
    [SerializeField] private GameObject arrowPrefab; // Assign in inspector

    public override void Attack(Soldier attacker, IDamageable target)
    {
        attacker.StartCoroutine(PerformAttack(attacker, target));
    }

    private IEnumerator PerformAttack(Soldier attacker, IDamageable target)
    {
        attacker.TriggerAttackAnimation();
        yield return new WaitForSeconds(0.5f); // Adjust to match animation timing

        if (target != null && target == attacker.CurrentTarget)
        {
            SpawnArrow(attacker, target);
        }

        yield return new WaitForSeconds(0.5f); // Adjust to match animation timing
        attacker.ResetAttackAnimation();
    }

    private void SpawnArrow(Soldier attacker, IDamageable target)
    {
        GameObject arrow = Instantiate(arrowPrefab, attacker.transform.position, Quaternion.identity);
        Arrow arrowScript = arrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.Initialize(target, attacker.Stats.Damage);
        }
    }
}