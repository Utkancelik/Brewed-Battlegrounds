using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Soldier))]
public class RangedAttack : IAttackBehavior
{
    [SerializeField] private GameObject projectilePrefab;
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
            SpawnProjectile(attacker, target.transform.position, attacker.IsEnemy);
        }

        yield return new WaitForSeconds(0.5f);
        attacker.ResetAttackAnimation();
        isAttacking = false;
    }

    private void SpawnProjectile(Soldier attacker, Vector3 targetPosition, bool isEnemy)
    {
        GameObject projectile = Instantiate(projectilePrefab, attacker.transform.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().Initialize(targetPosition, attacker.Data.Damage, isEnemy);
    }
}

