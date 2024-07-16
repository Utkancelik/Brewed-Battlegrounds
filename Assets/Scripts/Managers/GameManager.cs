using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] public Base PlayerBase;
    [SerializeField] public Base EnemyBase;
    [SerializeField] private SpawnArea playerSpawnArea;
    [SerializeField] private SpawnArea enemySpawnArea;
    [SerializeField] public List<SoldierType> soldierTypes; // List of soldier types, now public
    [SerializeField] private GameObject gameOverPanel; // Reference to the Game Over Panel

    private SoldierSpawner soldierSpawner;
    private ResourceManager resourceManager;
    private AgeManager ageManager;
    private int roundGoldEarned;

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
        // Ensure the first soldier type is unlocked at the start
        if (soldierTypes.Count > 0)
        {
            soldierTypes[0].isUnlocked = true;
        }

        UIManager.Instance.UpdateGoldUI(resourceManager.Gold); // Initialize the gold UI with the current gold amount
        UIManager.Instance.UpdateFoodUI(resourceManager.Food); // Initialize the food UI with the current food amount
        UIManager.Instance.UpdateTotalGoldUI(resourceManager.TotalGold); // Initialize the total gold UI with the total gold amount
        UIManager.Instance.CreateUnitButtons(soldierTypes); // Initialize unit buttons

        gameOverPanel.SetActive(false); // Ensure the Game Over panel is inactive at the start
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
        resourceManager.StartProduction();
    }

    public void CheckGameOver()
    {
        if (PlayerBase.Health <= 0 || EnemyBase.Health <= 0)
        {
            isBattleStarted = false;
            roundGoldEarned = CalculateRoundGold(); // Method to calculate gold earned in this round
            resourceManager.AddRoundGoldToTotal(); // Add round gold to total gold
            UIManager.Instance.ShowGameOverPanel(roundGoldEarned); // Show game over panel
        }
    }

    private int CalculateRoundGold()
    {
        // Implement your logic to calculate gold earned in this round
        return ResourceManager.Instance.RoundGold; // Return the round gold earned
    }

    public void AddGold(int amount)
    {
        ResourceManager.Instance.AddRoundGold(amount); // Add to round gold
        UIManager.Instance.UpdateGoldUI(ResourceManager.Instance.Gold);
        UIManager.Instance.UpdateRoundGoldUI(ResourceManager.Instance.RoundGold); // Update round gold UI
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
