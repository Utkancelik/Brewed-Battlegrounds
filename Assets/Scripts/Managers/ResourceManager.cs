using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject foodPrefab;
    public GameObject GoldPrefab => goldPrefab;
    public GameObject FoodPrefab => foodPrefab;

    [SerializeField] private int gold = 0;
    [SerializeField] private int food = 0;
    [SerializeField] private float foodProductionRate = 1f; // Food per second
    public int foodProductionUpgradeCost = 10;
    public int baseHealthUpgradeCost = 15;

    private float foodProductionTimer = 0f;
    private bool isBattleStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UIManager.Instance.UpdateGoldUI(gold);
        UIManager.Instance.UpdateFoodUI(food);
    }

    private void Update()
    {
        if (isBattleStarted)
        {
            ProduceResources();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UIManager.Instance.UpdateGoldUI(gold);
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UIManager.Instance.UpdateGoldUI(gold);
            return true;
        }
        return false;
    }

    public void AddFood(int amount)
    {
        food += amount;
        UIManager.Instance.UpdateFoodUI(food);
    }

    public void SpendFood(int amount)
    {
        food -= amount;
        UIManager.Instance.UpdateFoodUI(food);
    }

    public void ProduceResources()
    {
        foodProductionTimer += Time.deltaTime;
        if (foodProductionTimer >= 1f / foodProductionRate)
        {
            food += Mathf.FloorToInt(foodProductionTimer * foodProductionRate);
            foodProductionTimer = 0f;
            UIManager.Instance.UpdateFoodUI(food);
        }
    }

    public void StartProduction()
    {
        isBattleStarted = true;
    }

    public void IncreaseFoodProduction()
    {
        foodProductionRate += 0.2f;
        foodProductionUpgradeCost += 10; // Increment the upgrade cost
    }

    public void IncreaseBaseHealth()
    {
        GameManager.Instance.PlayerBase.IncreaseHealth();
        baseHealthUpgradeCost += 15; // Increment the upgrade cost
    }

    public int Gold => gold;
    public int Food => food;
}

