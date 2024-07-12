using UnityEngine;

public class Base : IDamageable
{
    [SerializeField] private int health;
    [SerializeField] private bool isEnemy;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private GameObject destructionEffectPrefab;
    [SerializeField] private GameObject goldPrefab;

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

    public override void Die()
    {
        if (destructionEffectPrefab != null)
        {
            Instantiate(destructionEffectPrefab, transform.position, Quaternion.identity);
        }

        if (isEnemy)
        {
            DropGold(); // Drop gold only if it is the enemy base
        }

        Destroy(gameObject);

        // Trigger game over or win condition based on which base died
        GameManager.Instance.CheckGameOver();
    }

    private void DropGold()
    {
        GameObject gold = Instantiate(goldPrefab, transform.position, Quaternion.identity);
        gold.GetComponent<Gold>().Initialize(Random.insideUnitCircle.normalized * 2f);
    }
}


