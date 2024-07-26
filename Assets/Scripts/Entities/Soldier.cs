using UnityEngine;
using UnityEngine.Serialization;

public class Soldier : IDamageable
{
    [FormerlySerializedAs("stats")]
    [FormerlySerializedAs("statsSo")]
    [Header("References")]
    [SerializeField] private SoldierDataSO data;
    [SerializeField] private int health;
    [SerializeField] private bool isEnemy;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject[] bloodTracePrefabs;

    private IAttackBehavior attackBehavior;
    private IMoveBehavior moveBehavior;
    private IDamageable attackTarget;
    private HealthBar healthBar;
    private Animator animator;
    private DamageFlicker damageFlicker;
    private Rigidbody2D rb;
    private bool isAttacking;
    private bool isDead;
    private float goldDropCooldown = 0.5f; // Cooldown in seconds
    private float goldDropTimer = 0;
    private Vector3 currentDirection;
    private float directionChangeInterval = 2f;
    private float directionChangeTimer;
    
    public IDamageable CurrentTarget { get; set; }
    public SoldierDataSO Data => data;
    public override int Health
    {
        get => health;
        set
        {
            health = value;
            healthBar.SetHealth(health, data.Health);
        }
    }
    public override bool IsEnemy
    {
        get => isEnemy;
        set => isEnemy = value;
    }

    private ResourceManager _resourceManager;
    private GameManager _gameManager;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        attackBehavior = GetComponent<IAttackBehavior>();
        moveBehavior = GetComponent<IMoveBehavior>();
        damageFlicker = GetComponent<DamageFlicker>();

        InitializeHealthBar();
    }

    private void InitializeHealthBar()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        healthBar.Initialize(transform, data.Health);
        healthBar.SetHealth(health, data.Health);
    }

    private void Start()
    {
        SetNewDirection();

        _resourceManager = FindObjectOfType<ResourceManager>();
        _gameManager = FindObjectOfType<GameManager>();
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
        goldDropTimer -= Time.deltaTime;  // Reduce cooldown timer

        if (directionChangeTimer <= 0)
        {
            SetNewDirection();
        }
    }

    private void EngageTarget()
    {
        if (!isAttacking)
        {
            Vector3 targetPosition = GetClosestPointOnTarget();
            if (Vector3.Distance(transform.position, targetPosition) > data.AttackRange)
            {
                MoveTowards(targetPosition);
            }
            else
            {
                FaceTarget();
                StopMovement();
                attackTarget = CurrentTarget;
                isAttacking = true;
                attackBehavior.Attack(this, attackTarget);

                // Drop gold if player soldier attacks enemy base
                if (!isEnemy && CurrentTarget is Base && goldDropTimer <= 0)
                {
                    DropGold(CurrentTarget.transform.position);
                    goldDropTimer = goldDropCooldown;  // Reset cooldown
                }
            }
        }

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
            MoveTowards(transform.position + currentDirection); // Patrol without moving diagonally
            isAttacking = false;
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        if (!isAttacking)
        {
            moveBehavior.Move(rb, targetPosition, data.Speed);
        }
    }

    private void SetNewDirection()
    {
        currentDirection = new Vector2(isEnemy ? -1 : 1, 0).normalized; // Set new direction without diagonal
        directionChangeTimer = directionChangeInterval;
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    private void DetectTargets()
    {
        if (isAttacking && !(CurrentTarget is Base)) return;

        IDamageable closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in Physics2D.OverlapCircleAll(transform.position, data.DetectRange))
        {
            IDamageable target = hitCollider.GetComponent<IDamageable>();
            if (target != null && target.IsEnemy != isEnemy && target.Health > 0)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                if (distanceToTarget < closestDistance)
                {
                    closestTarget = target;
                    closestDistance = distanceToTarget;
                }
            }
        }

        CurrentTarget = closestTarget ?? (isEnemy ? _gameManager.PlayerBase : _gameManager.EnemyBase);
    }

    private Vector3 GetClosestPointOnTarget()
    {
        if (CurrentTarget is Base baseTarget)
        {
            return baseTarget.GetComponent<Collider2D>().ClosestPoint(transform.position);
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

        if (isEnemy)
        {
            DropGold(transform.position);
        }

        ResetAttackAnimation();
        if (deathEffectPrefab != null)
        {
            Destroy(Instantiate(deathEffectPrefab, transform.position, Quaternion.identity), 1.5f);
        }

        if (bloodTracePrefabs.Length > 0)
        {
            GameObject bloodTrace = Instantiate(bloodTracePrefabs[Random.Range(0, bloodTracePrefabs.Length)], transform.position, Quaternion.identity);
            bloodTrace.AddComponent<BloodTraceFade>().StartFadeOut(15.0f);
        }

        Destroy(gameObject);
    }

    private void DropGold(Vector3 position)
    {
        GameObject gold = Instantiate(_resourceManager.goldPrefab, position, Quaternion.identity);
        gold.GetComponent<Gold>().Initialize(Random.insideUnitCircle.normalized * .75f);
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

    public void StopAllActions()
    {
        isAttacking = false;
        rb.velocity = Vector2.zero;
        ResetAttackAnimation();
        enabled = false;
    }
}
