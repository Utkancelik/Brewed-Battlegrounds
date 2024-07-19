using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    [SerializeField] public GameObject goldPrefab;
    [SerializeField] private GameObject foodPrefab;
    [SerializeField] private int gold = 0;
    [SerializeField] private int food = 0;
    [SerializeField] private int roundGold = 0;
    [SerializeField] private int totalGold = 0;
    [SerializeField] public float foodProductionRate = 0.5f;
    public int foodProductionUpgradeCost = 10;
    public int baseHealthUpgradeCost = 15;

    private float foodProductionTimer = 0f;
    private bool isBattleStarted = false;
    private Image foodFillingImage;

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
        foodFillingImage = UIManager.Instance.GetFoodFillingImage();
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
        totalGold = PlayerPrefs.GetInt("TotalGold", 0);
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
        foodFillingImage.fillAmount = foodProductionTimer * foodProductionRate;

        if (foodProductionTimer >= 1f / foodProductionRate)
        {
            food += Mathf.FloorToInt(foodProductionTimer * foodProductionRate);
            foodProductionTimer = 0f;
            UIManager.Instance.UpdateFoodUI(food);
            StartCoroutine(UIManager.Instance.FillFoodImage(foodFillingImage, 1f / foodProductionRate));
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
            PlayerPrefs.SetInt("BaseHealth", playerBase.maxHealth);
            PlayerPrefs.Save();
        }
    }

    public void LoadUpgrades()
    {
        foodProductionRate = PlayerPrefs.GetFloat("FoodProductionRate", 0.5f);
        UIManager.Instance.UpdateFoodProductionRateUI(foodProductionRate);
        int savedBaseHealth = PlayerPrefs.GetInt("BaseHealth", GameManager.Instance.PlayerBase.maxHealth);
        GameManager.Instance.PlayerBase.maxHealth = savedBaseHealth;
        GameManager.Instance.PlayerBase.Health = savedBaseHealth;
        UIManager.Instance.UpdateBaseHealthUI(savedBaseHealth);
    }
}
