using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class RangedAttack : IAttackBehavior
{
    [SerializeField] private GameObject arrowPrefab;
    private bool isAttacking;

    public override void Attack(Soldier attacker, IDamageable target)
    {
        if (!isAttacking)
        {
            attacker.StartCoroutine(PerformAttack(attacker, target));
        }
    }

    private IEnumerator PerformAttack(Soldier attacker, IDamageable target)
    {
        isAttacking = true;
        attacker.TriggerAttackAnimation();
        yield return new WaitForSeconds(0.5f);

        if (target != null)
        {
            SpawnArrow(attacker, target.transform.position);
        }

        yield return new WaitForSeconds(0.5f);
        attacker.ResetAttackAnimation();
        isAttacking = false;
    }

    private void SpawnArrow(Soldier attacker, Vector3 targetPosition)
    {
        GameObject arrow = Instantiate(arrowPrefab, attacker.transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().Initialize(targetPosition, attacker.Stats.Damage, attacker.gameObject);
    }
}









