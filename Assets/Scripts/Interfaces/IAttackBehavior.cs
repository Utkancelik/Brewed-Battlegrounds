using UnityEngine;

public abstract class IAttackBehavior : MonoBehaviour
{
    public abstract void Attack(Soldier attacker, IDamageable target);
}
