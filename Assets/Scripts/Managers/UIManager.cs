using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject waveTextObject;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private TMP_Text goldText; // Reference to the UI text element displaying gold
    [SerializeField] private TMP_Text foodText; // Reference to the UI text element displaying food
    [SerializeField] private GameObject unitButtonPrefab; // Prefab for unit buttons
    [SerializeField] private Transform unitButtonContainer; // Container for unit buttons
    public Button StartBattleButton => startBattleButton;

    [SerializeField] private GameObject mainBattlePanel; // Panel for the battle scene
    [SerializeField] private GameObject battleBottomPanel; // Panel for the battle controls
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject mainButtonsPanel; // Panel containing the 5 main buttons

    [SerializeField] private Button goToBattleButton; // Button to go to battle scene from main buttons
    [SerializeField] private Button upgradeButton; // Button to go to upgrade panel

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
        UpdateGoldUI(0); // Initialize the gold UI with 0 gold
        UpdateFoodUI(0); // Initialize the food UI with 0 food
        goToBattleButton.onClick.AddListener(EnterBattleScene);
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
        startBattleButton.onClick.AddListener(StartBattle);
        EnterBattleScene(); // Ensure the MainBattlePanel is active when the game starts
    }

    public void ShowMainButtonsPanel()
    {
        mainButtonsPanel.SetActive(true);
        mainBattlePanel.SetActive(false);
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void EnterBattleScene()
    {
        mainBattlePanel.SetActive(true);
        mainButtonsPanel.SetActive(true); // Keep the main buttons visible
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void OpenUpgradePanel()
    {
        mainBattlePanel.SetActive(false);
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(true);
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
}
