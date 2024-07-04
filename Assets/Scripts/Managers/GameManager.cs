using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SoldierStats PlayerInfantryStats { get; private set; }
    public SoldierStats EnemyInfantryStats { get; private set; }
    // Add similar properties for other soldier types

    public Transform playerBase;
    public Transform enemyBase;
    public int gold = 0;
    public int goldProductionRate = 1;
    private float goldTimer;

    public GameObject playerInfantryPrefab;
    public GameObject enemyInfantryPrefab;
    public GameObject playerArcherPrefab;
    public GameObject enemyArcherPrefab;
    public GameObject playerCavalryPrefab;
    public GameObject enemyCavalryPrefab;

    public SpawnArea playerSpawnArea;
    public SpawnArea enemySpawnArea;

    public List<WaveData> waveDataList;
    private int currentWaveIndex = 0;

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

        InitializeStats();
    }

    void InitializeStats()
    {
        PlayerInfantryStats = new SoldierStats(10, 2f, 5f, 1f); // Adjust detect and attack range values as needed
        EnemyInfantryStats = new SoldierStats(10, 2f, 5f, 1f);
        // Initialize other stats similarly
    }


    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        ProduceGold();

        if (Input.GetKeyDown(KeyCode.Alpha1) && gold >= 5)
        {
            SpawnSoldier(playerInfantryPrefab, false);
            gold -= 5;
        }
    }

    void ProduceGold()
    {
        goldTimer += Time.deltaTime;
        if (goldTimer >= 1f)
        {
            gold += goldProductionRate;
            goldTimer = 0f;
        }
    }

    public void UpgradePlayerInfantryDamage(int amount)
    {
        PlayerInfantryStats.UpgradeDamage(amount);
    }

    public void UpgradePlayerInfantrySpeed(float amount)
    {
        PlayerInfantryStats.UpgradeSpeed(amount);
    }

    public void UpgradePlayerInfantryAttackRange(float amount)
    {
        PlayerInfantryStats.UpgradeAttackRange(amount);
    }

    // Add similar methods for other upgrades

    IEnumerator SpawnWaves()
    {
        while (currentWaveIndex < waveDataList.Count)
        {
            WaveData waveData = waveDataList[currentWaveIndex];
            foreach (GameObject unit in waveData.units)
            {
                SpawnSoldier(unit, true);
                yield return new WaitForSeconds(waveData.delayBetweenUnits);
            }
            currentWaveIndex++;
            yield return new WaitForSeconds(10f); // Delay between waves
        }
    }

    void SpawnSoldier(GameObject soldierPrefab, bool isEnemy)
    {
        Vector3 spawnPosition = isEnemy ? enemySpawnArea.GetRandomPosition() : playerSpawnArea.GetRandomPosition();
        GameObject newSoldier = Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
    
        // Log to check the prefab being instantiated
        Debug.Log("Instantiated soldier: " + newSoldier.name);
    
        Soldier soldierScript = newSoldier.GetComponent<Soldier>();
        if (soldierScript == null)
        {
            Debug.LogError("Soldier component not found on instantiated prefab.");
        }
        soldierScript.IsEnemy = isEnemy;
        newSoldier.tag = isEnemy ? "EnemySoldier" : "PlayerSoldier";
    }
}