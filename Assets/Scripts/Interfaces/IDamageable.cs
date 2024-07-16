using UnityEngine;

using UnityEngine;

public abstract class IDamageable : MonoBehaviour
{
    public abstract int Health { get; set; }
    public abstract bool IsEnemy { get; set; }

    public abstract void TakeDamage(int damage);
    public abstract void Die();
}



