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
    [SerializeField] public List<SoldierType> soldierTypes;
    [SerializeField] private GameObject gameOverPanel;

    private SoldierSpawner soldierSpawner;
    private ResourceManager resourceManager;
    private int roundGoldEarned;
    private bool isBattleStarted = false;

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

        BattleManager.Instance.SetSoldierSpawner(soldierSpawner);
    }

    void Start()
    {
        if (soldierTypes.Count > 0)
        {
            soldierTypes[0].IsUnlocked = true;
        }

        UIManager.Instance.UpdateGoldUI(resourceManager.Gold);
        UIManager.Instance.UpdateFoodUI(resourceManager.Food);
        UIManager.Instance.UpdateTotalGoldUI(resourceManager.TotalGold);
        UIManager.Instance.CreateUnitButtons(soldierTypes);

        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (isBattleStarted)
        {
            resourceManager.ProduceResources();

            if (Input.GetKeyDown(KeyCode.Alpha1) && resourceManager.Food >= 5)
            {
                soldierSpawner.SpawnSoldier(soldierTypes[0].Prefab, false);
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
            roundGoldEarned = CalculateRoundGold();
            resourceManager.AddRoundGoldToTotal();
            UIManager.Instance.ShowGameOverPanel(roundGoldEarned);
        }
    }

    private int CalculateRoundGold()
    {
        return ResourceManager.Instance.RoundGold;
    }

    public void AddGold(int amount)
    {
        ResourceManager.Instance.AddRoundGold(amount);
        UIManager.Instance.UpdateGoldUI(ResourceManager.Instance.Gold);
        UIManager.Instance.UpdateRoundGoldUI(ResourceManager.Instance.RoundGold);
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
