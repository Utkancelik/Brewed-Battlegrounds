using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] public Base PlayerBase;
    [SerializeField] public Base EnemyBase;
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
        gold += amount;
        UIManager.Instance.UpdateGoldUI(gold);
    }

    public void SpendGold(int amount)
    {
        gold -= amount;
        UIManager.Instance.UpdateGoldUI(gold);
    }
}

