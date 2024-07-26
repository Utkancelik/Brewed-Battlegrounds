using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action OnBattleStarted;
    public static event Action OnGameOver;

    [SerializeField] public Base PlayerBase;
    [SerializeField] public Base EnemyBase;
    [SerializeField] private SpawnArea playerSpawnArea;
    [SerializeField] private SpawnArea enemySpawnArea;
    [SerializeField] public List<SoldierDataSO> allSoldierTypes;
    [SerializeField] private GameObject gameOverPanel;

    private SoldierSpawner _soldierSpawner;
    private ResourceManager _resourceManager;
    private UIManager _uiManager;
    private BattleManager _battleManager;

    private int roundGoldEarned;
    private bool isBattleStarted = false;
    private bool isGoldAddedToTotal = false;

    private void Awake()
    {
        _soldierSpawner = new SoldierSpawner(playerSpawnArea, enemySpawnArea);
    }

    private void Start()
    {
        _resourceManager = FindObjectOfType<ResourceManager>();
        _uiManager = FindObjectOfType<UIManager>();
        _battleManager = FindObjectOfType<BattleManager>();

        if (allSoldierTypes.Count > 0)
        {
            allSoldierTypes[0].IsUnlocked = true;
        }

        Debug.Log("GameManager Start: allSoldierTypes count: " + allSoldierTypes.Count);

        _battleManager.SetSoldierSpawner(_soldierSpawner);

        _uiManager.UpdateGoldUI(_resourceManager.Gold);
        _uiManager.UpdateFoodUI(_resourceManager.Food);
        _uiManager.UpdateTotalGoldUI(_resourceManager.TotalGold);

        Debug.Log("Calling UIManager.Initialize");
        _uiManager.Initialize(allSoldierTypes); // Ensure this is called after allSoldierTypes is populated

        gameOverPanel.SetActive(false);
    }

    private void Update()
    {
        if (isBattleStarted)
        {
            _resourceManager.ProduceResources();
        }
    }

    public void StartBattle()
    {
        isBattleStarted = true;
        _resourceManager.StartProduction();
        OnBattleStarted?.Invoke();
    }

    public void CheckGameOver()
    {
        if (PlayerBase.Health <= 0 || EnemyBase.Health <= 0)
        {
            isBattleStarted = false;
            StopAllActions();
            _battleManager.StopSpawning();
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
            _resourceManager.AddRoundGoldToTotal();
            isGoldAddedToTotal = true;
        }

        if (EnemyBase.Health <= 0)
        {
            _uiManager.ShowGameOverPanel(roundGoldEarned);
        }

        OnGameOver?.Invoke();
        _uiManager.FadeAndReload();
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
        return _resourceManager.RoundGold;
    }

    public void SpawnSoldier(SoldierDataSO soldierDataSo)
    {
        _soldierSpawner.SpawnSoldier(soldierDataSo.Prefab, false);
    }
}
