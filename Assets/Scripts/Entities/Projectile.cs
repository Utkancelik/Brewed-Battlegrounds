using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector3 initialTargetPosition;
    private int damage;
    [SerializeField] private float speed = 10f;
    private bool isEnemy;

    public void Initialize(Vector3 targetPosition, int damage, bool isEnemy)
    {
        this.initialTargetPosition = targetPosition;
        this.damage = damage;
        this.isEnemy = isEnemy;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.right = direction;
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
            if (target != null && target.IsEnemy != isEnemy)
            {
                target.TakeDamage(damage);
                Destroy(gameObject);
                break;
            }
        }
    }
}
