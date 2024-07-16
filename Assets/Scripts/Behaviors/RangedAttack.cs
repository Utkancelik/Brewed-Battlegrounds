using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class RangedAttack : IAttackBehavior
{
    [SerializeField] private GameObject arrowPrefab; // Assign in inspector
    private bool isAttacking; // Flag to indicate if the archer is currently attacking

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
            yield return new WaitForSeconds(0.5f); // Adjust to match animation timing

            if (target != null && target == attacker.CurrentTarget)
            {
                SpawnArrow(attacker, target);
            }

            yield return new WaitForSeconds(0.5f); // Adjust to match animation timing
            attacker.ResetAttackAnimation();
        }
        isAttacking = false; // Reset the flag after the attack finishes
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



