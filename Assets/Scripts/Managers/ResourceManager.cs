using System;
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
    
    public event Action<int> OnGoldChanged;
    public event Action<int> OnFoodChanged;
    public event Action<int> OnTotalGoldChanged;
    public event Action<int> OnRoundGoldChanged;
    public event Action<float> OnFoodProductionRateChanged;
    
    private void Start()
    {
        _uiManager = FindObjectOfType<UIManager>();
        _gameManager = FindObjectOfType<GameManager>();

        LoadTotalGold();
        _uiManager.UpdateGoldUI(gold);
        _uiManager.UpdateFoodUI(food);
        _uiManager.UpdateTotalGoldUI(totalGold);
        foodFillingImage = _uiManager.GetFoodFillingImage();
    }

    public void AddRoundGold(int amount)
    {
        roundGold += amount;
        OnRoundGoldChanged?.Invoke(roundGold);
    }

    public void AddRoundGoldToTotal()
    {
        totalGold += roundGold;
        OnTotalGoldChanged?.Invoke(totalGold);
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
            OnTotalGoldChanged?.Invoke(totalGold);
            return true;
        }
        return false;
    }

    public void SpendFood(int amount)
    {
        food -= amount;
        OnFoodChanged?.Invoke(food);
    }

    public void ProduceResources()
    {
        foodProductionTimer += Time.deltaTime;
        foodFillingImage.fillAmount = foodProductionTimer * foodProductionRate;

        if (foodProductionTimer >= 1f / foodProductionRate)
        {
            food += Mathf.FloorToInt(foodProductionTimer * foodProductionRate);
            foodProductionTimer = 0f;
            OnFoodChanged?.Invoke(food);
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
            OnFoodProductionRateChanged?.Invoke(foodProductionRate);
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
