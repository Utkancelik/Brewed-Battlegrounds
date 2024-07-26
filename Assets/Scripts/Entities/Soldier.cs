using UnityEngine;

public class Soldier : IDamageable
{
    [Header("References")]
    [SerializeField] private SoldierDataSO data;
    [SerializeField] private int health;
    [SerializeField] private bool isEnemy;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject[] bloodTracePrefabs;

    private ResourceManager _resourceManager;
    private GameManager _gameManager;

    private IAttackBehavior _attackBehavior;
    private IMoveBehavior _moveBehavior;
    private IDamageable _attackTarget;
    private HealthBar _healthBar;
    private Animator _animator;
    private DamageFlicker _damageFlicker;
    private Rigidbody2D _rb;
    private bool _isAttacking;
    private bool _isDead;
    private const float GoldDropCooldown = 0.5f; // Cooldown in seconds
    private float _goldDropTimer = 0;
    private Vector3 _currentDirection;

    public IDamageable CurrentTarget { get; private set; }
    public SoldierDataSO Data => data;
    public override int Health
    {
        get => health;
        set
        {
            health = value;
            _healthBar.SetHealth(health, data.Health);
        }
    }
    public override bool IsEnemy
    {
        get => isEnemy;
        set => isEnemy = value;
    }

    private static readonly int Attack = Animator.StringToHash("Attack");

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        _attackBehavior = GetComponent<IAttackBehavior>();
        _moveBehavior = GetComponent<IMoveBehavior>();
        _damageFlicker = GetComponent<DamageFlicker>();

        InitializeHealthBar();
    }

    private void InitializeHealthBar()
    {
        _healthBar = GetComponentInChildren<HealthBar>();
        _healthBar.Initialize(transform, data.Health);
        _healthBar.SetHealth(health, data.Health);
    }

    private void Start()
    {
        _resourceManager = FindObjectOfType<ResourceManager>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (_isDead) return;

        if (CurrentTarget != null && CurrentTarget.Health > 0)
        {
            EngageTarget();
        }
        else
        {
            PatrolArea();
        }

        _goldDropTimer -= Time.deltaTime;  // Reduce cooldown timer
    }

    private void EngageTarget()
    {
        if (!_isAttacking)
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
                _attackTarget = CurrentTarget;
                _isAttacking = true;
                _attackBehavior.Attack(this, _attackTarget);

                // Drop gold if player soldier attacks enemy base
                if (!isEnemy && CurrentTarget is Base && _goldDropTimer <= 0)
                {
                    DropGold(CurrentTarget.transform.position);
                    _goldDropTimer = GoldDropCooldown;  // Reset cooldown
                }
            }
        }

        DetectTargets();
        if (CurrentTarget != _attackTarget)
        {
            _isAttacking = false;
        }
    }

    private void PatrolArea()
    {
        DetectTargets();
        if (CurrentTarget == null)
        {
            MoveTowards(transform.position + _currentDirection); // Patrol without moving diagonally
            _isAttacking = false;
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        if (!_isAttacking)
        {
            _moveBehavior.Move(_rb, targetPosition, data.Speed);
        }
    }

    private void StopMovement()
    {
        _rb.velocity = Vector2.zero;
    }

    private void DetectTargets()
    {
        if (_isAttacking && !(CurrentTarget is Base)) return;

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
        _animator?.SetTrigger(Attack);
    }

    public void ResetAttackAnimation()
    {
        _animator?.ResetTrigger(Attack);
        _isAttacking = false;
    }

    public override void Die()
    {
        if (_isDead) return;
        _isDead = true;

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
        if (_isDead) return;

        Health -= damage;
        _damageFlicker?.Flicker(0.1f, Color.gray);
        if (Health <= 0)
        {
            Die();
        }
    }

    public void StopAllActions()
    {
        _isAttacking = false;
        _rb.velocity = Vector2.zero;
        ResetAttackAnimation();
        enabled = false;
    }
}
