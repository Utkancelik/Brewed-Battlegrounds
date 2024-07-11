using UnityEngine;

public class Base : IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private bool isEnemy;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject destructionEffectPrefab;
    [SerializeField] private Transform parentTransform;  // Reference to the parent object

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

    private int maxHealth;

    private void Awake()
    {
        InitializeHealthBar();
        maxHealth = health;
        parentTransform = transform.parent; // Set the parent transform
    }

    private void InitializeHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.Initialize(transform);
            healthBar.SetMaxHealth(health);
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

    public override void Die()
    {
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject); // Destroy the parent object

        // Trigger game over or win condition based on which base died
        GameManager.Instance.CheckGameOver();
    }
}

