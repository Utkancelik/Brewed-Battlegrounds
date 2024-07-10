using UnityEngine;

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

        BattleManager.Instance.SetSoldierSpawner(soldierSpawner);
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
}