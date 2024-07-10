using UnityEngine;
public class Soldier : MonoBehaviour
{
    [SerializeField] private SoldierStats stats;
    [SerializeField] private int health;
    public int Health
    {
        get => health;
        set
        {
            health = value;
            healthBar.SetHealth(health, stats.Health);
        }
    }

    [SerializeField] private bool isEnemy;
    public bool IsEnemy
    {
        get => isEnemy;
        set => isEnemy = value;
    }

    private IMoveBehavior moveBehavior;
    private IAttackBehavior attackBehavior;
    private HealthBar healthBar;
    private Animator animator;
    public Soldier currentTarget;
    private bool isAttacking;
    private bool isDead = false;

    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject[] bloodTracePrefabs;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        moveBehavior = GetComponent<IMoveBehavior>();
        attackBehavior = GetComponent<IAttackBehavior>();

        ValidateComponents();

        InitializeHealthBar();
    }

    private void ValidateComponents()
    {
        if (moveBehavior == null) Debug.LogError("MoveBehavior not assigned on " + gameObject.name);
        if (attackBehavior == null) Debug.LogError("AttackBehavior not assigned on " + gameObject.name);
        if (stats == null) Debug.LogError("Stats not assigned on " + gameObject.name);
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
        if (currentTarget != null && currentTarget.Health > 0)
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
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget > stats.AttackRange)
        {
            MoveTowards(currentTarget.transform.position);
        }
        else if (!isAttacking)
        {
            FaceTarget();
            StopMovement();
            attackBehavior.Attack(this, currentTarget);
            isAttacking = true;
        }
    }

    private void PatrolArea()
    {
        DetectEnemy();
        if (currentTarget == null)
        {
            FaceEnemyBase();
            moveBehavior.Move(GetComponent<Rigidbody2D>(), IsEnemy, stats.Speed);
            isAttacking = false;
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        GetComponent<Rigidbody2D>().velocity = direction * stats.Speed;
    }

    private void StopMovement()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
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
                transform.localScale = new Vector3(0.8f, transform.localScale.y, transform.localScale.z);
            }
            else if (currentTarget.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-0.8f, transform.localScale.y, transform.localScale.z);
            }
        }
    }

    private void FaceEnemyBase()
    {
        if (IsEnemy)
        {
            transform.localScale = new Vector3(-0.8f, transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(0.8f, transform.localScale.y, transform.localScale.z);
        }
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
                currentTarget = null;
                ResetAttackAnimation();
            }
        }
    }

    public void Die()
    {
        if (isDead) return; // Prevent multiple calls to Die
        isDead = true;

        ResetAttackAnimation();

        // Instantiate death effect
        if (deathEffectPrefab != null)
        {
            Destroy(Instantiate(deathEffectPrefab, transform.position, Quaternion.identity), 1.5f);
        }

        // Instantiate blood trace and start coroutine to fade it out
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

