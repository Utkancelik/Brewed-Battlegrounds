using UnityEngine;

public class Soldier : MonoBehaviour
{
    [SerializeField] private SoldierStats stats;
    [SerializeField] private int health;
    [SerializeField] private bool isEnemy;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject[] bloodTracePrefabs;

    private IMoveBehavior moveBehavior;
    private IAttackBehavior attackBehavior;
    private HealthBar healthBar;
    private Animator animator;
    private DamageFlicker damageFlicker;
    private Rigidbody2D rb;
    private bool isAttacking;
    private bool isDead;

    public Soldier CurrentTarget { get; set; }
    public SoldierStats Stats => stats;
    public int Health
    {
        get => health;
        set
        {
            health = value;
            healthBar.SetHealth(health, stats.Health);
        }
    }
    public bool IsEnemy
    {
        get => isEnemy;
        set => isEnemy = value;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        moveBehavior = GetComponent<IMoveBehavior>();
        attackBehavior = GetComponent<IAttackBehavior>();
        damageFlicker = GetComponent<DamageFlicker>();

        ValidateComponents();
        InitializeHealthBar();
    }

    private void ValidateComponents()
    {
        if (moveBehavior == null) Debug.LogError("MoveBehavior not assigned on " + gameObject.name);
        if (attackBehavior == null) Debug.LogError("AttackBehavior not assigned on " + gameObject.name);
        if (stats == null) Debug.LogError("Stats not assigned on " + gameObject.name);
        if (damageFlicker == null) Debug.LogError("DamageFlicker not assigned on " + gameObject.name);
    }

    private void InitializeHealthBar()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(transform);
            healthBar.SetMaxHealth(stats.Health);
        }
        else
        {
            Debug.LogError("HealthBar component not found in children of " + gameObject.name);
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (CurrentTarget != null && CurrentTarget.Health > 0)
        {
            EngageTarget();
        }
        else
        {
            PatrolArea();
        }
    }

    private void EngageTarget()
    {
        if (!isAttacking)
        {
            float distanceToTarget = Vector3.Distance(transform.position, CurrentTarget.transform.position);
            if (distanceToTarget > stats.AttackRange)
            {
                MoveTowards(CurrentTarget.transform.position);
            }
            else
            {
                FaceTarget();
                StopMovement();
                attackBehavior.Attack(this, CurrentTarget);
                isAttacking = true;
            }
        }
    }

    private void PatrolArea()
    {
        DetectEnemy();
        if (CurrentTarget == null)
        {
            FaceEnemyBase();
            moveBehavior.Move(rb, IsEnemy, stats.Speed);
            isAttacking = false;
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        rb.velocity = direction * stats.Speed;
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    private void DetectEnemy()
    {
        if (isAttacking) return;

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

        CurrentTarget = closestTarget;
    }

    private void FaceTarget()
    {
        if (CurrentTarget == null) return;

        transform.localScale = CurrentTarget.transform.position.x > transform.position.x
            ? new Vector3(0.8f, transform.localScale.y, transform.localScale.z)
            : new Vector3(-0.8f, transform.localScale.y, transform.localScale.z);
    }

    private void FaceEnemyBase()
    {
        transform.localScale = IsEnemy
            ? new Vector3(-0.8f, transform.localScale.y, transform.localScale.z)
            : new Vector3(0.8f, transform.localScale.y, transform.localScale.z);
    }

    public void TriggerAttackAnimation()
    {
        animator?.SetTrigger("Attack");
    }

    public void ResetAttackAnimation()
    {
        animator?.ResetTrigger("Attack");
        isAttacking = false;
    }

    public void DealDamage(Soldier target)
    {
        if (target != null && target.Health > 0)
        {
            target.TakeDamage(stats.Damage);
            if (target.Health <= 0)
            {
                target.Die();
                CurrentTarget = null;
                ResetAttackAnimation();
            }
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        ResetAttackAnimation();

        if (deathEffectPrefab != null)
        {
            Destroy(Instantiate(deathEffectPrefab, transform.position, Quaternion.identity), 1.5f);
        }

        if (bloodTracePrefabs.Length != 0)
        {
            GameObject bloodTrace = Instantiate(bloodTracePrefabs[Random.Range(0, bloodTracePrefabs.Length)], transform.position, Quaternion.identity);
            bloodTrace.AddComponent<BloodTraceFade>().StartFadeOut(5.0f);
        }

        Destroy(gameObject);
    }

    public void OnAttackAnimationEnd()
    {
        ResetAttackAnimation();
    }

    public virtual void TakeDamage(int damage)
    {
        Health -= damage;
        damageFlicker?.Flicker(0.1f, Color.gray);
        if (Health <= 0)
        {
            Die();
        }
    }
}
