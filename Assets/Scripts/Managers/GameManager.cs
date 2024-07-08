using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private Transform playerBase;
    [SerializeField] private Transform enemyBase;
    [SerializeField] private WaveData waveData;
    [SerializeField] private SpawnArea playerSpawnArea;
    [SerializeField] private SpawnArea enemySpawnArea;
    [SerializeField] private GameObject unitPrefab; // General unit prefab

    private SoldierSpawner soldierSpawner;
    [SerializeField] private GoldManager goldManager;
    private AgeManager ageManager;

    [SerializeField] private int gold;

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
            soldierSpawner.SpawnSoldier(unitPrefab, false);
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
                GameObject enemyGameObject = Instantiate(waveData.Waves[i].Soldier.gameObject, enemySpawnArea.GetRandomPosition(), Quaternion.identity);
                Soldier enemySoldier = enemyGameObject.GetComponent<Soldier>();
                enemySoldier.IsEnemy = true;
                yield return new WaitForSeconds(waveData.delayBetweenUnits);
            }
            yield return new WaitForSeconds(waveData.delayBetweenWaves); // Delay between waves
        }
    }
}
