using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{
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
    
    private UIManager _uiManager;
    private GameManager _gameManager;
    

    private void Awake()
    {
        DIContainer.Instance.Register(this);
    }

    private void Start()
    {
        _uiManager = DIContainer.Instance.Resolve<UIManager>();
        _gameManager = DIContainer.Instance.Resolve<GameManager>();
        
        LoadTotalGold();
        _uiManager.UpdateGoldUI(gold);
        _uiManager.UpdateFoodUI(food);
        _uiManager.UpdateTotalGoldUI(totalGold);
        foodFillingImage = _uiManager.GetFoodFillingImage();
    }
    public void AddRoundGold(int amount)
    {
        roundGold += amount;
        _uiManager.UpdateRoundGoldUI(roundGold);
    }

    public void AddRoundGoldToTotal()
    {
        totalGold += roundGold;
        _uiManager.UpdateTotalGoldUI(totalGold);
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
            _uiManager.UpdateTotalGoldUI(totalGold);
            SaveTotalGold();
            return true;
        }
        return false;
    }

    public void SpendFood(int amount)
    {
        food -= amount;
        _uiManager.UpdateFoodUI(food);
    }

    public void ProduceResources()
    {
        foodProductionTimer += Time.deltaTime;
        foodFillingImage.fillAmount = foodProductionTimer * foodProductionRate;

        if (foodProductionTimer >= 1f / foodProductionRate)
        {
            food += Mathf.FloorToInt(foodProductionTimer * foodProductionRate);
            foodProductionTimer = 0f;
            _uiManager.UpdateFoodUI(food);
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
            foodProductionRate += 0.1f;
            _uiManager.UpdateFoodProductionRateUI(foodProductionRate);
            PlayerPrefs.SetFloat("FoodProductionRate", foodProductionRate);
            PlayerPrefs.Save();
            _uiManager.UpdateUpgradeButtonsUI();
        }
    }

    public void IncreaseBaseHealth()
    {
        if (SpendGold(baseHealthUpgradeCost))
        {
            Base playerBase = _gameManager.PlayerBase;
            playerBase.maxHealth += 50;
            playerBase.Health = playerBase.maxHealth;
            _uiManager.UpdateBaseHealthUI(playerBase.Health);
            PlayerPrefs.SetInt("BaseHealth", playerBase.maxHealth);
            PlayerPrefs.Save();
            _uiManager.UpdateUpgradeButtonsUI();
        }
    }

    public void LoadUpgrades()
    {
        foodProductionRate = PlayerPrefs.GetFloat("FoodProductionRate", .5f);
        _uiManager.UpdateFoodProductionRateUI(foodProductionRate);
        int savedBaseHealth = PlayerPrefs.GetInt("BaseHealth", _gameManager.PlayerBase.maxHealth);
        _gameManager.PlayerBase.maxHealth = savedBaseHealth;
        _gameManager.PlayerBase.Health = savedBaseHealth;
        _uiManager.UpdateBaseHealthUI(savedBaseHealth);
    }
}
