using System;
using UnityEngine;

public abstract class Soldier : MonoBehaviour
{
    protected SoldierStats stats;
    public int Health { get; set; }
    public bool IsEnemy { get; set; }

    protected IMoveBehavior moveBehavior;
    protected IAttackBehavior attackBehavior;
    private Animator animator;
    public Soldier currentTarget;

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
            else
            {
                // Attack the target
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                attackBehavior.Attack(this, currentTarget);
            }
        }
        else
        {
            // Move towards the enemy base
            moveBehavior.Move(GetComponent<Rigidbody2D>(), IsEnemy, stats.Speed);
        }

        DetectEnemy();
    }

    void DetectEnemy()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, stats.DetectRange);
        foreach (var hitCollider in hitColliders)
        {
            Soldier target = hitCollider.GetComponent<Soldier>();
            if (target != null && target.IsEnemy != IsEnemy && target.Health > 0)
            {
                currentTarget = target;
                break;
            }
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
    }

    // Method to deal damage called by the animation event
    public void DealDamage()
    {
        if (currentTarget != null && currentTarget.Health > 0)
        {
            Debug.Log("Dealing damage to " + currentTarget.name);
            currentTarget.Health -= GetStats().Damage;
            if (currentTarget.Health <= 0)
            {
                GameObject.Destroy(currentTarget.gameObject);
                currentTarget = null; // Clear the target
            }
        }
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