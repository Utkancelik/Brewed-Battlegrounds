using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject waveTextObject;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private GameObject unitButtonPrefab;
    [SerializeField] private Transform unitButtonContainer;
    public Button StartBattleButton => startBattleButton;

    [SerializeField] private GameObject mainBattlePanel;
    [SerializeField] private GameObject battleBottomPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject mainButtonsPanel;

    [SerializeField] private Button goToBattleButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button increaseFoodProductionButton;
    [SerializeField] private Button increaseBaseHealthButton;

    [SerializeField] private List<Button> unlockSoldierButtons;
    [SerializeField] private List<Image> soldierCards;

    private List<Button> unitButtons = new List<Button>();

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
        UpdateGoldUI(0);
        UpdateFoodUI(0);
        goToBattleButton.onClick.AddListener(EnterBattleScene);
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
        startBattleButton.onClick.AddListener(StartBattle);
        increaseFoodProductionButton.onClick.AddListener(IncreaseFoodProduction);
        increaseBaseHealthButton.onClick.AddListener(IncreaseBaseHealth);

        GameManager.Instance.soldierTypes[0].isUnlocked = true;

        for (int i = 0; i < unlockSoldierButtons.Count; i++)
        {
            int index = i;
            unlockSoldierButtons[i].onClick.AddListener(() => UnlockSoldierType(index));
        }

        EnterBattleScene();
        UpdateUpgradePanel();
    }

    public void ShowMainButtonsPanel()
    {
        mainButtonsPanel.SetActive(true);
        mainBattlePanel.SetActive(true);
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void EnterBattleScene()
    {
        mainBattlePanel.SetActive(true);
        mainButtonsPanel.SetActive(true);
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void OpenUpgradePanel()
    {
        mainBattlePanel.SetActive(false);
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(true);
        UpdateUpgradePanel();
    }

    public void StartBattle()
    {
        mainButtonsPanel.SetActive(false);
        battleBottomPanel.SetActive(true);
    }

    public void DisplayWaveText(string text)
    {
        waveTextObject.GetComponentInChildren<TMP_Text>().text = text;
        waveTextObject.SetActive(true);
    }

    public void HideWaveText()
    {
        waveTextObject.SetActive(false);
    }

    public void UpdateGoldUI(int goldAmount)
    {
        if (goldText != null)
        {
            goldText.text = $"{goldAmount}";
        }
        else
        {
            Debug.LogError("Gold text UI element not assigned in UIManager.");
        }
    }

    public void UpdateFoodUI(int foodAmount)
    {
        if (foodText != null)
        {
            foodText.text = $"{foodAmount}";
        }
        else
        {
            Debug.LogError("Food text UI element not assigned in UIManager.");
        }
    }

    public Vector3 GetGoldUIPosition()
    {
        Vector3 screenPosition = goldText.transform.position;
        return screenPosition;
    }

    public void CreateUnitButtons(List<SoldierType> soldierTypes)
    {
        ClearUnitButtons();

        foreach (var soldierType in soldierTypes)
        {
            if (!soldierType.isUnlocked) continue;

            GameObject buttonObj = Instantiate(unitButtonPrefab, unitButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            unitButtons.Add(button);

            TMP_Text[] texts = buttonObj.GetComponentsInChildren<TMP_Text>();
            Image unitImage = buttonObj.transform.Find("UnitImage").GetComponent<Image>();
            Image unitCostImage = buttonObj.transform.Find("UnitCostImage").GetComponent<Image>();
            TMP_Text unitCostText = unitCostImage.transform.Find("UnitCostText").GetComponent<TMP_Text>();

            if (texts.Length > 0)
            {
                texts[0].text = soldierType.soldierName;
            }
            if (unitImage != null)
            {
                unitImage.sprite = soldierType.soldierIcon;
            }
            if (unitCostText != null)
            {
                unitCostText.text = soldierType.stats.FoodCost.ToString();
            }

            button.onClick.AddListener(() => SpawnSoldier(soldierType));
        }
    }

    private void ClearUnitButtons()
    {
        foreach (var button in unitButtons)
        {
            Destroy(button.gameObject);
        }
        unitButtons.Clear();
    }

    private void SpawnSoldier(SoldierType soldierType)
    {
        if (ResourceManager.Instance.Food >= soldierType.stats.FoodCost)
        {
            GameManager.Instance.SpawnSoldier(soldierType);
            ResourceManager.Instance.SpendFood(soldierType.stats.FoodCost);
        }
    }

    private void UnlockSoldierType(int index)
    {
        if (index < 0 || index >= GameManager.Instance.soldierTypes.Count) return;

        SoldierType soldierType = GameManager.Instance.soldierTypes[index];
        if (!soldierType.isUnlocked && ResourceManager.Instance.SpendGold(soldierType.unlockCost))
        {
            soldierType.isUnlocked = true;
            UpdateUpgradePanel();
            CreateUnitButtons(GameManager.Instance.soldierTypes);
        }
    }

    private void IncreaseFoodProduction()
    {
        if (ResourceManager.Instance.SpendGold(ResourceManager.Instance.foodProductionUpgradeCost))
        {
            ResourceManager.Instance.IncreaseFoodProduction();
            UpdateUpgradePanel();
        }
    }

    private void IncreaseBaseHealth()
    {
        if (ResourceManager.Instance.SpendGold(ResourceManager.Instance.baseHealthUpgradeCost))
        {
            GameManager.Instance.PlayerBase.IncreaseHealth();
            UpdateUpgradePanel();
        }
    }

    private void UpdateUpgradePanel()
    {
        for (int i = 0; i < soldierCards.Count; i++)
        {
            if (i < GameManager.Instance.soldierTypes.Count)
            {
                SoldierType soldierType = GameManager.Instance.soldierTypes[i];
                soldierCards[i].color = soldierType.isUnlocked ? Color.white : Color.gray;
                unlockSoldierButtons[i].gameObject.SetActive(!soldierType.isUnlocked || i == 0);
                unlockSoldierButtons[i].GetComponentInChildren<TMP_Text>().text = soldierType.isUnlocked ? "Unlocked" : $"{soldierType.unlockCost} Gold";
            }
            else
            {
                soldierCards[i].gameObject.SetActive(false);
            }
        }

        // Update the food production and base health upgrade buttons
        increaseFoodProductionButton.GetComponentInChildren<TMP_Text>().text = $"Increase Food Production (+0.2/s) ({ResourceManager.Instance.foodProductionUpgradeCost} Gold)";
        increaseBaseHealthButton.GetComponentInChildren<TMP_Text>().text = $"Increase Base Health (+100) ({ResourceManager.Instance.baseHealthUpgradeCost} Gold)";
    }
}
