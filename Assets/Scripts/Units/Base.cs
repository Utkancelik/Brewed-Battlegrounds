using UnityEngine;

public class Base : IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private bool isEnemy;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject destructionEffectPrefab;
    [SerializeField] private GameObject goldPrefab;

    private int maxHealth;

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

    private void Awake()
    {
        maxHealth = health;
        InitializeHealthBar();
    }

    private void InitializeHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.Initialize(transform, maxHealth);
            healthBar.SetHealth(maxHealth, maxHealth); // Ensure health bar is updated at the start
        }
        else
        {
            Debug.LogError("HealthBar component not found on " + gameObject.name);
        }
    }

    public override void TakeDamage(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    public void IncreaseHealth()
    {
        maxHealth += 100; // Increase max health by 100
        Health = maxHealth; // Restore health to new max
        healthBar.SetHealth(Health, maxHealth); // Update health bar
    }

    public override void Die()
    {
        Destroy(gameObject);
        GameManager.Instance.CheckGameOver();
    }
}
