using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] public Base PlayerBase;
    [SerializeField] public Base EnemyBase;
    [SerializeField] private SpawnArea playerSpawnArea;
    [SerializeField] private SpawnArea enemySpawnArea;
    [SerializeField] private GameObject unitPrefab; // General unit prefab

    private SoldierSpawner soldierSpawner;
    private AgeManager ageManager;

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
        ageManager = new AgeManager();

        BattleManager.Instance.SetSoldierSpawner(soldierSpawner);
    }

    void Start()
    {
        UIManager.Instance.UpdateGoldUI(ResourceManager.Instance.Gold); // Initialize the gold UI with the current gold amount
        UIManager.Instance.UpdateFoodUI(ResourceManager.Instance.Food); // Initialize the food UI with the current food amount
    }

    void Update()
    {
        if (ResourceManager.Instance.isBattleStarted)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && ResourceManager.Instance.Food >= 5)
            {
                soldierSpawner.SpawnSoldier(unitPrefab, false);
                ResourceManager.Instance.SpendFood(5);
            }
        }
    }

    public void StartBattle()
    {
        ResourceManager.Instance.StartBattle();
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
        ResourceManager.Instance.AddGold(amount);
        UIManager.Instance.UpdateGoldUI(ResourceManager.Instance.Gold);
    }

    public void SpendGold(int amount)
    {
        ResourceManager.Instance.SpendGold(amount);
        UIManager.Instance.UpdateGoldUI(ResourceManager.Instance.Gold);
    }

    public void AddFood(int amount)
    {
        ResourceManager.Instance.AddFood(amount);
        UIManager.Instance.UpdateFoodUI(ResourceManager.Instance.Food);
    }

    public void SpendFood(int amount)
    {
        ResourceManager.Instance.SpendFood(amount);
        UIManager.Instance.UpdateFoodUI(ResourceManager.Instance.Food);
    }
}

