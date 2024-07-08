using System;
using UnityEngine;

using UnityEngine;

public abstract class Soldier : MonoBehaviour
{
    [SerializeField] protected SoldierStats stats;
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

    protected IMoveBehavior moveBehavior;
    protected IAttackBehavior attackBehavior;
    private Animator animator;
    public Soldier currentTarget;
    private bool isAttacking;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
        }
        else
        {
            Debug.Log("Animator assigned on " + gameObject.name);
        }

        InitializeStatsAndBehaviors();
    }

    private void InitializeStatsAndBehaviors()
    {
        SetMoveBehavior(new NormalMove());
        SetAttackBehavior(new MeleeAttack());
    }

    public void SetMoveBehavior(IMoveBehavior mb)
    {
        moveBehavior = mb;
    }

    public void SetAttackBehavior(IAttackBehavior ab)
    {
        attackBehavior = ab;
    }

    void Update()
    {
        if (currentTarget != null && currentTarget.Health > 0)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (distanceToTarget > stats.AttackRange)
            {
                // Move towards the current target
                Vector3 direction = (currentTarget.transform.position - transform.position).normalized;
                GetComponent<Rigidbody2D>().velocity = direction * stats.Speed;
            }
            else if (!isAttacking)
            {
                // Face the target
                FaceTarget();

                // Attack the target
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                attackBehavior.Attack(this, currentTarget);
                isAttacking = true;
            }
        }
        else
        {
            // Re-evaluate the target
            DetectEnemy();

            // Move towards the enemy base with collision avoidance
            if (currentTarget == null)
            {
                FaceEnemyBase();
                moveBehavior.Move(GetComponent<Rigidbody2D>(), IsEnemy, stats.Speed);
                isAttacking = false;
            }
        }
    }

    void DetectEnemy()
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

    void FaceTarget()
    {
        if (currentTarget != null)
        {
            // Flip the soldier based on the target's position using the transform's scale
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

    void FaceEnemyBase()
    {
        // Flip the soldier based on the direction towards the enemy base
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
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            Debug.LogError("Animator is not assigned on " + gameObject.name);
        }
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
            Debug.Log("Dealing damage to " + target.name);
            target.Health -= GetStats().Damage;
            if (target.Health <= 0)
            {
                target.Die();
                currentTarget = null; // Clear the target
                ResetAttackAnimation();
            }
        }
    }

    public void Die()
    {
        // Handle the death of the soldier
        Debug.Log(name + " has died.");
        ResetAttackAnimation();
        GameObject.Destroy(gameObject);
    }

    // This method will be called by the animation event to reset the animation
    public void OnAttackAnimationEnd()
    {
        Debug.Log("OnAttackAnimationEnd called on " + gameObject.name);
        ResetAttackAnimation();
    }

    public abstract void Display();

    public SoldierStats GetStats()
    {
        return stats;
    }
}
