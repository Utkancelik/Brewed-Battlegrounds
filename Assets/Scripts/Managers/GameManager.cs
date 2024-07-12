using UnityEngine;
using UnityEngine.SceneManagement; // Add this to handle scene management
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] public Base PlayerBase;
    [SerializeField] public Base EnemyBase;
    [SerializeField] private SpawnArea playerSpawnArea;
    [SerializeField] private SpawnArea enemySpawnArea;
    [SerializeField] private List<SoldierType> soldierTypes; // List of soldier types

    private SoldierSpawner soldierSpawner;
    private ResourceManager resourceManager;
    private AgeManager ageManager;

    private bool isBattleStarted = false; // Track if the battle has started

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
        resourceManager = ResourceManager.Instance;
        ageManager = new AgeManager();

        BattleManager.Instance.SetSoldierSpawner(soldierSpawner);
    }

    void Start()
    {
        UIManager.Instance.UpdateGoldUI(resourceManager.Gold); // Initialize the gold UI with the current gold amount
        UIManager.Instance.UpdateFoodUI(resourceManager.Food); // Initialize the food UI with the current food amount
        UIManager.Instance.CreateUnitButtons(soldierTypes); // Initialize unit buttons
    }

    void Update()
    {
        if (isBattleStarted)
        {
            resourceManager.ProduceResources();

            if (Input.GetKeyDown(KeyCode.Alpha1) && resourceManager.Food >= 5)
            {
                soldierSpawner.SpawnSoldier(soldierTypes[0].Prefab, false); // Ensure soldierTypes[0] exists
                resourceManager.SpendFood(5);
            }
        }
    }

    public void StartBattle()
    {
        isBattleStarted = true;
    }

    public void CheckGameOver()
    {
        if (PlayerBase.Health <= 0)
        {
            // Handle player loss
            Debug.Log("Player lost!");
            ReloadScene(); // Reload the scene on player loss
        }
        else if (EnemyBase.Health <= 0)
        {
            // Handle player win
            Debug.Log("Player won!");
            // Additional logic for game over
        }
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void AddGold(int amount)
    {
        resourceManager.AddGold(amount);
        UIManager.Instance.UpdateGoldUI(resourceManager.Gold);
    }

    public void SpendGold(int amount)
    {
        resourceManager.SpendGold(amount);
        UIManager.Instance.UpdateGoldUI(resourceManager.Gold);
    }

    public void AddFood(int amount)
    {
        resourceManager.AddFood(amount);
        UIManager.Instance.UpdateFoodUI(resourceManager.Food);
    }

    public void SpendFood(int amount)
    {
        resourceManager.SpendFood(amount);
        UIManager.Instance.UpdateFoodUI(resourceManager.Food);
    }

    public void SpawnSoldier(SoldierType soldierType)
    {
        soldierSpawner.SpawnSoldier(soldierType.Prefab, false);
    }
}
