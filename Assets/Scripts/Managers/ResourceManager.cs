using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [SerializeField] private GameObject goldPrefab;
    [SerializeField] private GameObject foodPrefab;
    public GameObject GoldPrefab => goldPrefab;
    public GameObject FoodPrefab => foodPrefab;

    [SerializeField] private int gold = 0;
    [SerializeField] private int food = 0;
    [SerializeField] private int roundGold = 0;
    [SerializeField] private int totalGold = 0;
    [SerializeField] private float foodProductionRate = 1f;
    public int foodProductionUpgradeCost = 10;
    public int baseHealthUpgradeCost = 15;

    private float foodProductionTimer = 0f;
    private bool isBattleStarted = false;

    public int TotalGold => totalGold;
    public int RoundGold => roundGold;
    public int Gold => gold;
    public int Food => food;

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
    }

    private void Start()
    {
        UIManager.Instance.UpdateGoldUI(gold);
        UIManager.Instance.UpdateFoodUI(food);
    }

    private void Update()
    {
        if (isBattleStarted)
        {
            ProduceResources();
        }
    }

    public void AddGold(int amount)
    {
        gold += amount;
        UIManager.Instance.UpdateGoldUI(gold);
    }

    public void AddRoundGold(int amount)
    {
        roundGold += amount;
        UIManager.Instance.UpdateRoundGoldUI(roundGold);
    }

    public void AddRoundGoldToTotal()
    {
        totalGold += roundGold;
        roundGold = 0;
        UIManager.Instance.UpdateTotalGoldUI(totalGold);
        UIManager.Instance.UpdateRoundGoldUI(roundGold);
    }

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            UIManager.Instance.UpdateGoldUI(gold);
            return true;
        }
        return false;
    }

    public void AddFood(int amount)
    {
        food += amount;
        UIManager.Instance.UpdateFoodUI(food);
    }

    public void SpendFood(int amount)
    {
        food -= amount;
        UIManager.Instance.UpdateFoodUI(food);
    }

    public void ProduceResources()
    {
        foodProductionTimer += Time.deltaTime;
        if (foodProductionTimer >= 1f / foodProductionRate)
        {
            food += Mathf.FloorToInt(foodProductionTimer * foodProductionRate);
            foodProductionTimer = 0f;
            UIManager.Instance.UpdateFoodUI(food);
        }
    }

    public void StartProduction()
    {
        isBattleStarted = true;
    }
}

