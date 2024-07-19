using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private bool isGoldAddedToTotal = false;

    public static event Action OnBattleStarted;
    public static event Action OnGameOver;

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

        soldierSpawner = new SoldierSpawner(playerSpawnArea, enemySpawnArea);
        resourceManager = ResourceManager.Instance;
    }

    private void Start()
    {
        if (soldierTypes.Count > 0)
        {
            soldierTypes[0].IsUnlocked = true;
        }
    
        BattleManager.Instance.SetSoldierSpawner(soldierSpawner);
        
        UIManager.Instance.UpdateGoldUI(resourceManager.Gold);
        UIManager.Instance.UpdateFoodUI(resourceManager.Food);
        UIManager.Instance.UpdateTotalGoldUI(resourceManager.TotalGold);
        UIManager.Instance.CreateUnitButtons(soldierTypes);

        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (isBattleStarted)
        {
            resourceManager.ProduceResources();
        }
    }

    public void StartBattle()
    {
        isBattleStarted = true;
        resourceManager.StartProduction();
        OnBattleStarted?.Invoke();
    }

    public void CheckGameOver()
    {
        if (PlayerBase.Health <= 0 || EnemyBase.Health <= 0)
        {
            isBattleStarted = false;
            StopAllActions();
            BattleManager.Instance.StopSpawning();
            CollectAllGold();
            StartCoroutine(WaitForAllGoldToBeCollected());
        }
    }

    private void CollectAllGold()
    {
        var allGold = FindObjectsOfType<Gold>();
        foreach (var gold in allGold)
        {
            gold.MoveImmediately();
        }
    }

    private IEnumerator WaitForAllGoldToBeCollected()
    {
        while (FindObjectsOfType<Gold>().Length > 0)
        {
            yield return null;
        }

        if (!isGoldAddedToTotal)
        {
            roundGoldEarned = CalculateRoundGold();
            resourceManager.AddRoundGoldToTotal();
            isGoldAddedToTotal = true;
        }

        if (EnemyBase.Health <= 0)
        {
            UIManager.Instance.ShowGameOverPanel(roundGoldEarned);
        }

        OnGameOver?.Invoke();
        UIManager.Instance.FadeAndReload();
    }

    private void StopAllActions()
    {
        var allSoldiers = FindObjectsOfType<Soldier>();
        foreach (var soldier in allSoldiers)
        {
            soldier.StopAllActions();
        }
        StopAllCoroutines();
    }

    private int CalculateRoundGold()
    {
        return ResourceManager.Instance.RoundGold;
    }

    public void SpawnSoldier(SoldierType soldierType)
    {
        soldierSpawner.SpawnSoldier(soldierType.Prefab, false);
    }
}
