using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Vector3 initialTargetPosition;
    private int damage;
    private float speed = 10f;
    private GameObject shooter; // Reference to the shooter to ignore collisions

    public void Initialize(Vector3 targetPosition, int damage, GameObject shooter)
    {
        this.initialTargetPosition = targetPosition;
        this.damage = damage;
        this.shooter = shooter;

        // Calculate direction based on the initial target position
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.right = direction;

        if (shooter != null)
        {
            // Ignore collision with the shooter
            Collider2D shooterCollider = shooter.GetComponent<Collider2D>();
            if (shooterCollider != null)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), shooterCollider);
            }
        }
    }

    private void Update()
    {
        transform.position += transform.right * speed * Time.deltaTime;
        CheckForCollision();
    }

    private void CheckForCollision()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (var hitCollider in hitColliders)
        {
            IDamageable target = hitCollider.GetComponent<IDamageable>();
            if (target != null && shooter != null && target.IsEnemy != shooter.GetComponent<IDamageable>().IsEnemy)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
                break;
            }
        }
    }
}
