using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Collections;

using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private SoldierStats playerInfantryStats;
    [SerializeField] private SoldierStats enemyInfantryStats;
    [SerializeField] private Transform playerBase;
    [SerializeField] private Transform enemyBase;
    [SerializeField] private WaveData waveData;
    [SerializeField] private SpawnArea playerSpawnArea;
    [SerializeField] private SpawnArea enemySpawnArea;
    [SerializeField] private GameObject playerInfantryPrefab;

    private SoldierSpawner soldierSpawner;
    [SerializeField] private GoldManager goldManager;
    private AgeManager ageManager;

    [SerializeField] private int gold;  // Define the gold variable

    public SoldierStats PlayerInfantryStats => playerInfantryStats;
    public SoldierStats EnemyInfantryStats => enemyInfantryStats;

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
        soldierSpawner = new SoldierSpawner(playerSpawnArea, enemySpawnArea);
        goldManager = new GoldManager();
        ageManager = new AgeManager();
    }

    void InitializeStats()
    {
        playerInfantryStats = new SoldierStats(10, 2f, 5f, 1f); // Adjust detect and attack range values as needed
        enemyInfantryStats = new SoldierStats(10, 2f, 5f, 1f);
    }

    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        goldManager.ProduceGold(ref gold);

        if (Input.GetKeyDown(KeyCode.Alpha1) && gold >= 5)
        {
            soldierSpawner.SpawnSoldier(playerInfantryPrefab, false);
            gold -= 5;
        }
    }

    IEnumerator SpawnWaves()
    {
        int waveCount = waveData.Waves.Count;
        for (int i = 0; i < waveCount; i++)
        {
            int totalEnemyInThisWave = waveData.Waves[i].Amount;
            for (int j = 0; j < totalEnemyInThisWave; j++)
            {
                Soldier whatSoldierEnemyWillSpawn = waveData.Waves[i].Soldier;
                GameObject enemyGameObject = whatSoldierEnemyWillSpawn.gameObject;
                soldierSpawner.SpawnSoldier(enemyGameObject, true);
                yield return new WaitForSeconds(waveData.delayBetweenUnits);
            }
            yield return new WaitForSeconds(waveData.delayBetweenWaves); // Delay between waves
        }
    }
}
