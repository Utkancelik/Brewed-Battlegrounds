using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] public Base PlayerBase;
    [SerializeField] public Base EnemyBase;
    [SerializeField] private WaveData waveData;
    [SerializeField] private SpawnArea playerSpawnArea;
    [SerializeField] private SpawnArea enemySpawnArea;
    [SerializeField] private GameObject unitPrefab; // General unit prefab

    private SoldierSpawner soldierSpawner;
    [SerializeField] private GoldManager goldManager;
    private AgeManager ageManager;
    [SerializeField] private int gold;
    [SerializeField] private int food;
    [SerializeField] private float foodProductionRate = 1f; // Food per second

    private float foodProductionTimer = 0f;

    void Awake()
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

        soldierSpawner = new SoldierSpawner(playerSpawnArea, enemySpawnArea);
        goldManager = new GoldManager();
        ageManager = new AgeManager();

        BattleManager.Instance.SetSoldierSpawner(soldierSpawner);
    }

    void Start()
    {
        UIManager.Instance.UpdateGoldUI(gold); // Initialize the gold UI with the current gold amount
        UIManager.Instance.UpdateFoodUI(food); // Initialize the food UI with the current food amount
    }

    void Update()
    {
        goldManager.ProduceGold(ref gold);
        ProduceFood();

        if (Input.GetKeyDown(KeyCode.Alpha1) && food >= 5)
        {
            soldierSpawner.SpawnSoldier(unitPrefab, false);
            SpendFood(5);
        }
    }

    public void CheckGameOver()
    {
        if (PlayerBase.Health <= 0)
        {
            // Handle player loss
            Debug.Log("Player lost!");
            // Additional logic for game over
        }
        else if (EnemyBase.Health <= 0)
        {
            // Handle player win
            Debug.Log("Player won!");
            // Additional logic for game over
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UIManager.Instance.UpdateGoldUI(gold);
    }

    public void SpendGold(int amount)
    {
        gold -= amount;
        UIManager.Instance.UpdateGoldUI(gold);
    }

    private void ProduceFood()
    {
        foodProductionTimer += Time.deltaTime;
        if (foodProductionTimer >= 1f / foodProductionRate)
        {
            food += Mathf.FloorToInt(foodProductionTimer * foodProductionRate);
            foodProductionTimer = 0f;
            UIManager.Instance.UpdateFoodUI(food);
        }
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
}

