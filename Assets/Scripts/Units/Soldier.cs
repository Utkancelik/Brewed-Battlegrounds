using System.Collections;
using UnityEngine;

public class Soldier : IDamageable
{
    [SerializeField] private SoldierStats stats;
    [SerializeField] private int health;
    [SerializeField] private bool isEnemy;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject[] bloodTracePrefabs;

    private IAttackBehavior attackBehavior;
    private IMoveBehavior moveBehavior;
    private HealthBar healthBar;
    private Animator animator;
    private DamageFlicker damageFlicker;
    private Rigidbody2D rb;
    private bool isAttacking;
    private bool isDead;

    private Vector3 currentDirection;
    private float directionChangeInterval = 2f; // Time to change direction in seconds
    private float directionChangeTimer;
    private IDamageable attackTarget;

    private Vector3 lastPosition;
    private float stuckTimer;
    private float stuckThreshold = 1f; // Time to consider a soldier as stuck

    public IDamageable CurrentTarget { get; set; }
    public SoldierStats Stats => stats;
    public override int Health
    {
        get => health;
        set
        {
            health = value;
            healthBar.SetHealth(health, stats.Health);
        }
    }
    public override bool IsEnemy
    {
        get => isEnemy;
        set => isEnemy = value;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        attackBehavior = GetComponent<IAttackBehavior>();
        moveBehavior = GetComponent<IMoveBehavior>();
        damageFlicker = GetComponent<DamageFlicker>();

        ValidateComponents();
        InitializeHealthBar();
    }

    private void ValidateComponents()
    {
        if (attackBehavior == null) Debug.LogError("AttackBehavior not assigned on " + gameObject.name);
        if (moveBehavior == null) Debug.LogError("MoveBehavior not assigned on " + gameObject.name);
        if (stats == null) Debug.LogError("Stats not assigned on " + gameObject.name);
        if (damageFlicker == null) Debug.LogError("DamageFlicker not assigned on " + gameObject.name);
    }

    private void InitializeHealthBar()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        if (healthBar != null)
        {
            healthBar.Initialize(transform, stats.Health);
            healthBar.SetHealth(health, stats.Health); // Set initial health display
        }
        else
        {
            Debug.LogError("HealthBar component not found in children of " + gameObject.name);
        }
    }

    private void Start()
    {
        SetNewDirection();
        lastPosition = transform.position;
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

        directionChangeTimer -= Time.deltaTime;
        if (directionChangeTimer <= 0)
        {
            SetNewDirection();
        }

        CheckIfStuck();
    }

    private void EngageTarget()
    {
        if (!isAttacking)
        {
            Vector3 targetPosition = GetClosestPointOnTarget();
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            if (distanceToTarget > stats.AttackRange)
            {
                MoveTowards(targetPosition);
            }
            else
            {
                FaceTarget();
                StopMovement();
                attackTarget = CurrentTarget; // Lock onto the current target
                isAttacking = true;
                attackBehavior.Attack(this, attackTarget); // Use attack behavior
            }
        }

        // Check for nearby enemies while engaging the base
        DetectTargets();
        if (CurrentTarget != attackTarget)
        {
            isAttacking = false;
        }
    }

    private void PatrolArea()
    {
        DetectTargets();
        if (CurrentTarget == null)
        {
            MoveDiagonally();
            isAttacking = false;
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        moveBehavior.Move(rb, targetPosition, stats.Speed);
    }

    private void MoveDiagonally()
    {
        if (!isAttacking)
        {
            if (moveBehavior != null)
            {
                moveBehavior.Move(rb, currentDirection, stats.Speed);
            }
        }
    }

    private void SetNewDirection()
    {
        Vector2 baseDirection = IsEnemy ? Vector2.left : Vector2.right;
        currentDirection = new Vector2(baseDirection.x, Random.Range(-0.4f, 0.4f)).normalized;
        directionChangeTimer = directionChangeInterval;
        MoveDiagonally(); // Ensure immediate movement in the new direction
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    private void DetectTargets()
    {
        if (isAttacking && !(CurrentTarget is Base)) return;

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, stats.DetectRange);
        IDamageable closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            IDamageable target = hitCollider.GetComponent<IDamageable>();
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

        // Prioritize enemy soldiers over bases
        if (closestTarget != null)
        {
            CurrentTarget = closestTarget;
        }
        else
        {
            // No enemy soldiers detected, target the opposing base
            if (IsEnemy)
            {
                CurrentTarget = GameManager.Instance.PlayerBase;
            }
            else
            {
                CurrentTarget = GameManager.Instance.EnemyBase;
            }
        }
    }

    private Vector3 GetClosestPointOnTarget()
    {
        if (CurrentTarget is Base baseTarget)
        {
            Collider2D baseCollider = baseTarget.GetComponent<Collider2D>();
            Vector3 closestPoint = baseCollider.ClosestPoint(transform.position);
            return closestPoint;
        }
        return CurrentTarget.transform.position;
    }

    private void FaceTarget()
    {
        if (CurrentTarget == null) return;

        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * (CurrentTarget.transform.position.x > transform.position.x ? 1 : -1);
        transform.localScale = newScale;
    }

    private void CheckIfStuck()
    {
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        if (distanceMoved < 0.1f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > stuckThreshold)
            {
                SetNewDirection();
                stuckTimer = 0;
            }
        }
        else
        {
            stuckTimer = 0;
        }
        lastPosition = transform.position;
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

    public override void Die()
    {
        if (isDead) return;
        isDead = true;

        // Drop gold on death if the soldier is an enemy
        if (IsEnemy)
        {
            DropGold();
        }

        ResetAttackAnimation();

        if (deathEffectPrefab != null)
        {
            Destroy(Instantiate(deathEffectPrefab, transform.position, Quaternion.identity), 1.5f);
        }

        if (bloodTracePrefabs.Length != 0)
        {
            GameObject bloodTrace = Instantiate(bloodTracePrefabs[Random.Range(0, bloodTracePrefabs.Length)], transform.position, Quaternion.identity);
            bloodTrace.AddComponent<BloodTraceFade>().StartFadeOut(15.0f);
        }

        Destroy(gameObject);
    }

    private void DropGold()
    {
        GameObject gold = Instantiate(ResourceManager.Instance.GoldPrefab, transform.position, Quaternion.identity);
        gold.GetComponent<Gold>().Initialize(Random.insideUnitCircle.normalized * 2f);
    }



    public override void TakeDamage(int damage)
    {
        if (isDead) return;

        Health -= damage;
        damageFlicker?.Flicker(0.1f, Color.gray);
        if (Health <= 0)
        {
            Die();
        }
    }
}
