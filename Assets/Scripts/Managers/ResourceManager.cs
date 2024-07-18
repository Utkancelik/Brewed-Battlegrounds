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
    [SerializeField] public float foodProductionRate = 1f;
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
        LoadTotalGold(); 
        UIManager.Instance.UpdateGoldUI(gold);
        UIManager.Instance.UpdateFoodUI(food);
        UIManager.Instance.UpdateTotalGoldUI(totalGold);
    }


    private void Update()
    {
        if (isBattleStarted)
        {
            ProduceResources();
        }
    }

    public void AddRoundGold(int amount)
    {
        roundGold += amount;
        UIManager.Instance.UpdateRoundGoldUI(roundGold);
    }

    public void AddRoundGoldToTotal()
    {
        totalGold += roundGold;
        UIManager.Instance.UpdateTotalGoldUI(totalGold);
        SaveTotalGold();
    }


    public void SaveTotalGold()
    {
        PlayerPrefs.SetInt("TotalGold", totalGold);
        PlayerPrefs.Save();
    }
    public void LoadTotalGold()
    {
        totalGold = PlayerPrefs.GetInt("TotalGold", 0); // 0 is the default value if not found
    }

    public bool SpendGold(int amount)
    {
        if (totalGold >= amount)
        {
            totalGold -= amount;
            UIManager.Instance.UpdateTotalGoldUI(totalGold);
            SaveTotalGold();
            return true;
        }
        return false;
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
    
    public void IncreaseFoodProductionRate()
    {
        if (SpendGold(foodProductionUpgradeCost))
        {
            foodProductionRate += 0.25f;
            UIManager.Instance.UpdateFoodProductionRateUI(foodProductionRate);

            // Save new food production rate
            PlayerPrefs.SetFloat("FoodProductionRate", foodProductionRate);
            PlayerPrefs.Save();
        }
    }

    public void IncreaseBaseHealth()
    {
        if (SpendGold(baseHealthUpgradeCost))
        {
            Base playerBase = GameManager.Instance.PlayerBase;
            playerBase.maxHealth += 50;
            playerBase.Health = playerBase.maxHealth;
            UIManager.Instance.UpdateBaseHealthUI(playerBase.Health);

            // Save new base health
            PlayerPrefs.SetInt("BaseHealth", playerBase.maxHealth);
            PlayerPrefs.Save();
        }
    }

    public void LoadUpgrades()
    {
        foodProductionRate = PlayerPrefs.GetFloat("FoodProductionRate", 1f);
        UIManager.Instance.UpdateFoodProductionRateUI(foodProductionRate);

        int savedBaseHealth = PlayerPrefs.GetInt("BaseHealth", GameManager.Instance.PlayerBase.maxHealth);
        GameManager.Instance.PlayerBase.maxHealth = savedBaseHealth;
        GameManager.Instance.PlayerBase.Health = savedBaseHealth;
        UIManager.Instance.UpdateBaseHealthUI(savedBaseHealth);
    }


}

