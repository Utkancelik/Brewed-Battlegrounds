using System;
using UnityEngine;

public class Base : IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private bool isEnemy;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject destructionEffectPrefab;
    [SerializeField] private GameObject destructionEffectPosition;

    public int maxHealth;

    public override int Health
    {
        get => health;
        set
        {
            health = value;
            healthBar.SetHealth(health, maxHealth);
        }
    }

    public override bool IsEnemy
    {
        get => isEnemy;
        set => isEnemy = value;
    }

    private GameManager _gameManager;
    private void Awake()
    {
        DIContainer.Instance.Register(this);
        maxHealth = health;
        InitializeHealthBar();
    }

    private void Start()
    {
        _gameManager = DIContainer.Instance.Resolve<GameManager>();
    }

    private void InitializeHealthBar()
    {
        healthBar.Initialize(transform, maxHealth);
        healthBar.SetHealth(maxHealth, maxHealth);
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }
    
    public override void Die()
    {
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }
        _gameManager.CheckGameOver();
        Destroy(gameObject);
    }


}
