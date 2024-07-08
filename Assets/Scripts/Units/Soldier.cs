using UnityEngine;

public class Soldier : MonoBehaviour
{
    [SerializeField] private SoldierStats stats;
    [SerializeField] private int health;
    public int Health
    {
        get { return health; }
        set { health = value; }
    }

    [SerializeField] private bool isEnemy;
    public bool IsEnemy
    {
        get { return isEnemy; }
        set { isEnemy = value; }
    }

    private IMoveBehavior moveBehavior;
    private IAttackBehavior attackBehavior;

    private Animator animator;
    public Soldier currentTarget;
    private bool isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        moveBehavior = GetComponent<IMoveBehavior>();
        attackBehavior = GetComponent<IAttackBehavior>();

        if (moveBehavior == null)
        {
            Debug.LogError("MoveBehavior not assigned on " + gameObject.name);
        }

        if (attackBehavior == null)
        {
            Debug.LogError("AttackBehavior not assigned on " + gameObject.name);
        }

        if (stats == null)
        {
            Debug.LogError("Stats not assigned on " + gameObject.name);
        }
    }

    private void Update()
    {
        if (currentTarget != null && currentTarget.Health > 0)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distanceToTarget > stats.AttackRange)
            {
                Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
                GetComponent<Rigidbody2D>().velocity = direction * stats.Speed;
            }
            else if (!isAttacking)
            {
                FaceTarget();
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                attackBehavior.Attack(this, currentTarget);
                isAttacking = true;
            }
        }
        else
        {
            DetectEnemy();
            if (currentTarget == null)
            {
                FaceEnemyBase();
                moveBehavior.Move(GetComponent<Rigidbody2D>(), IsEnemy, stats.Speed);
                isAttacking = false;
            }
        }
    }

    private void DetectEnemy()
    {
        if (!isAttacking)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, stats.DetectRange);
            Soldier closestTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (var hitCollider in hitColliders)
            {
                Soldier target = hitCollider.GetComponent<Soldier>();
                if (target != null && target.IsEnemy != IsEnemy && target.Health > 0)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestTarget = target;
                        closestDistance = distanceToTarget;
                    }
                }
            }

            currentTarget = closestTarget;
        }
    }

    private void FaceTarget()
    {
        if (currentTarget != null)
        {
            if (currentTarget.transform.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
            else if (currentTarget.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void FaceEnemyBase()
    {
        if (IsEnemy)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }

    public void TriggerAttackAnimation()
    {
        animator?.SetTrigger("Attack");
    }

    public void ResetAttackAnimation()
    {
        if (animator != null)
        {
            animator.ResetTrigger("Attack");
        }
        isAttacking = false;
    }

    public void DealDamage(Soldier target)
    {
        if (target != null && target.Health > 0)
        {
            target.Health -= stats.Damage;
            if (target.Health <= 0)
            {
                target.Die();
                currentTarget = null;
                ResetAttackAnimation();
            }
        }
    }

    public void Die()
    {
        ResetAttackAnimation();
        Destroy(gameObject);
    }

    public void OnAttackAnimationEnd()
    {
        ResetAttackAnimation();
    }

    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    public SoldierStats Stats
    {
        get => stats;
        set => stats = value;
    }
}
