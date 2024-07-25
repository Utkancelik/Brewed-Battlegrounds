using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static event Action OnStartBattle;
    
    [Header("General UI Elements")]
    [SerializeField] private GameObject waveTextObject;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private TMP_Text currentGoldText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text totalGoldVaultText;
    [SerializeField] private TMP_Text gameOverRoundGoldText;
    [SerializeField] private TMP_Text roundGoldInGameText;
    [SerializeField] private GameObject unitButtonPrefab;
    [SerializeField] private Transform unitButtonContainer;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text versionText;
    public Button StartBattleButton => startBattleButton;
    
    [Header("Panel Elements")]
    [SerializeField] private GameObject mainBattlePanel;
    [SerializeField] private GameObject battleBottomPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Image fadeOverlay;
    
    [Header("Button Elements")]
    [SerializeField] private Button goToBattleButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button closeGameOverPanelButton;
    [SerializeField] private Button increaseFoodProductionButton;
    [SerializeField] private Button increaseBaseHealthButton;

    [Header("Soldier Card Elements")]
    [SerializeField] private GameObject soldierCardPrefab;
    [SerializeField] private Transform soldiersContainer;
    [SerializeField] private List<SoldierCardUI> soldierCardsUI = new List<SoldierCardUI>();
    
    [Header("Resource Management UI")]
    [SerializeField] private TMP_Text foodProductionRateText;
    [SerializeField] private TMP_Text baseHealthText;
    [SerializeField] private Image foodFillingImage;
    [SerializeField] private TMP_Text foodProductionCostText;
    [SerializeField] private TMP_Text baseHealthCostText;
    private bool isFoodProcessed = false;
    
    [Header("Managers")]
    private ResourceManager _resourceManager;
    private GameManager _gameManager;

    [Header("Game State")]
    private List<SoldierDataSO> soldierTypes;
    private int currentEra = 1;

    [Header("Unit Buttons")]
    private List<Button> unitButtons = new List<Button>();

    private void OnEnable()
    {
        OnStartBattle += StartBattle;
        EraManager.OnEraChanged += UpdateSoldierTypesForEra;
    }

    private void OnDisable()
    {
        OnStartBattle -= StartBattle;
        EraManager.OnEraChanged -= UpdateSoldierTypesForEra;
    }
    
    private void Awake()
    {
        DIContainer.Instance.Register(this);
        versionText.text = $"V{Application.version}";
        Debug.Log(versionText.text);
    }

    
    private void Start()
    {
        _resourceManager = DIContainer.Instance.Resolve<ResourceManager>();
        _gameManager = DIContainer.Instance.Resolve<GameManager>();

        foodProductionCostText.text = _resourceManager.foodProductionUpgradeCost.ToString();
        baseHealthCostText.text = _resourceManager.baseHealthUpgradeCost.ToString();

        increaseFoodProductionButton.onClick.AddListener(() => 
        {
            if (_resourceManager.TotalGold >= _resourceManager.foodProductionUpgradeCost)
            {
                _resourceManager.IncreaseFoodProductionRate();
                UpdateUpgradeButtonsUI();
            }
        });

        increaseBaseHealthButton.onClick.AddListener(() => 
        {
            if (_resourceManager.TotalGold >= _resourceManager.baseHealthUpgradeCost)
            {
                _resourceManager.IncreaseBaseHealth();
                UpdateUpgradeButtonsUI();
            }
        });

        goToBattleButton.onClick.AddListener(EnterBattleScene);
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
        startBattleButton.onClick.AddListener(() => OnStartBattle?.Invoke());
        closeGameOverPanelButton.onClick.AddListener(CloseGameOverPanel);

        UpdateFoodProductionRateUI(_resourceManager.foodProductionRate);
        UpdateBaseHealthUI(_gameManager.PlayerBase.Health);
        UpdateGoldUI(_resourceManager.Gold);
        UpdateFoodUI(_resourceManager.Food);
        UpdateTotalGoldUI(_resourceManager.TotalGold);
        UpdateRoundGoldUI(_resourceManager.RoundGold);

        if (_gameManager.allSoldierTypes.Count > 0)
        {
            _gameManager.allSoldierTypes[0].IsUnlocked = true;
        }

        foreach (var soldierType in _gameManager.allSoldierTypes)
        {
            soldierType.IsUnlocked = PlayerPrefs.GetInt(soldierType.SoldierName, 0) == 1;
        }

        EnterBattleScene();
        _resourceManager.LoadUpgrades();

        gameOverPanel.SetActive(false);
        fadeOverlay.gameObject.SetActive(false);

        Initialize(_gameManager.allSoldierTypes);
    }
    public void Initialize(List<SoldierDataSO> allSoldierTypes)
    {
        soldierTypes = allSoldierTypes;
        CreateSoldierCards();
        UpdateSoldierTypesForEra(currentEra);
    }

    private void CreateSoldierCards()
    {
        if (soldierCardPrefab == null)
        {
            Debug.LogError("SoldierCardPrefab is not assigned in the inspector.");
            return;
        }

        foreach (Transform child in soldiersContainer)
        {
            Destroy(child.gameObject);
        }

        soldierCardsUI.Clear();

        var currentEraSoldiers = soldierTypes.Where(s => s.Era == currentEra).ToList();

        if (currentEraSoldiers.Count == 0)
        {
            Debug.LogWarning($"No soldier types found for the current era: {currentEra}");
            return;
        }

        for (int i = 0; i < currentEraSoldiers.Count; i++)
        {
            GameObject cardObj = Instantiate(soldierCardPrefab, soldiersContainer);
            SoldierCardUI soldierCardUI = cardObj.GetComponent<SoldierCardUI>();
            if (soldierCardUI != null)
            {
                soldierCardsUI.Add(soldierCardUI);
                soldierCardUI.Setup(currentEraSoldiers[i], _resourceManager, this);
            }
            else
            {
                Debug.LogError("SoldierCardUI component is missing on the instantiated prefab.");
            }
        }
    }
    
    public void UpdateUpgradeButtonsUI()
    {
        UpdateUpgradeButtonColor(foodProductionCostText, _resourceManager.foodProductionUpgradeCost, increaseFoodProductionButton);
        UpdateUpgradeButtonColor(baseHealthCostText, _resourceManager.baseHealthUpgradeCost, increaseBaseHealthButton);

        if (soldierCardsUI == null || soldierCardsUI.Count == 0)
        {
            Debug.LogError("soldierCardsUI is null or empty in UpdateUpgradeButtonsUI.");
            return;
        }

        for (int i = 0; i < soldierCardsUI.Count; i++)
        {
            if (i < soldierTypes.Count)
            {
                SoldierCardUI soldierCardUI = soldierCardsUI[i];
                SoldierDataSO soldierType = soldierTypes[i];
                Transform costTransform = soldierCardUI.transform.Find("Cost");

                if (costTransform != null && costTransform.gameObject.activeSelf)
                {
                    TMP_Text costText = costTransform.GetComponent<TMP_Text>();
                    costText.text = soldierType.UnlockCost.ToString();
                    UpdateUpgradeButtonColor(costText, soldierType.UnlockCost, null);
                }
                else
                {
                    Debug.LogWarning($"Cost component is not found or inactive for soldier: {soldierType.SoldierName}");
                }

                soldierCardUI.Setup(soldierType, _resourceManager, this);
            }
        }
    }
    private void UpdateUpgradeButtonColor(TMP_Text costText, int cost, Button associatedButton)
    {
        if (_resourceManager.TotalGold >= cost)
        {
            costText.color = Color.green;
            if (associatedButton != null)
            {
                associatedButton.interactable = true;
            }
        }
        else
        {
            costText.color = Color.red;
            if (associatedButton != null)
            {
                associatedButton.interactable = false;
            }
        }
    }


    private void UpdateSoldierTypesForEra(int era)
    {
        if (soldierTypes == null)
        {
            Debug.LogError("SoldierTypes list is null!");
            return;
        }

        currentEra = era;
        var eraSoldierTypes = soldierTypes.Where(s => s.Era == currentEra).ToList();

        if (eraSoldierTypes == null || eraSoldierTypes.Count == 0)
        {
            Debug.LogWarning("No soldier types found for the current era: " + currentEra);
        }

        UpdateUpgradePanel(eraSoldierTypes);
        CreateUnitButtons(eraSoldierTypes);
    }

    public void UpdateFoodProductionRateUI(float rate)
    {
        foodProductionRateText.text = $"Food Production Rate: {rate}/s";
    }

    public void UpdateBaseHealthUI(int health)
    {
        baseHealthText.text = $"Base Health: {health}";
    }

    public void EnterBattleScene()
    {
        statusText.text = $"Battle";
        mainBattlePanel.SetActive(true);
        mainButtonsPanel.SetActive(true);
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void OpenUpgradePanel()
    {
        statusText.text = $"Upgrades";
        mainBattlePanel.SetActive(false);
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(true);
        UpdateSoldierTypesForEra(currentEra);
        UpdateUpgradePanel(soldierTypes);
        UpdateUpgradeButtonsUI();
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
        StartCoroutine(SlideUIElement(waveTextObject.gameObject,500, -1));
    }

    public void UpdateGoldUI(int goldAmount)
    {
        currentGoldText.text = $"{goldAmount}";
    }

    public void UpdateTotalGoldUI(int totalGoldAmount)
    {
        if (totalGoldAmount >= 1000)
        {
            totalGoldVaultText.text = $"{totalGoldAmount / 1000f:0.#}k";
        }
        else
        {
            totalGoldVaultText.text = $"{totalGoldAmount}";
        }
    }


    public void UpdateFoodUI(int foodAmount)
    {
        foodText.text = $"{foodAmount}";
    }

    public void UpdateRoundGoldUI(int roundGoldAmount)
    {
        roundGoldInGameText.text = $"{roundGoldAmount}";
    }

    public void UpdateGameOverRoundGoldUI(int roundGoldAmount)
    {
        gameOverRoundGoldText.text = $"Gold Earned: {roundGoldAmount}";
    }

    public Vector3 GetGoldUIPosition()
    {
        return currentGoldText.transform.position;
    }

    public void CreateUnitButtons(List<SoldierDataSO> soldierTypes)
    {
        ClearUnitButtons();

        foreach (var soldierType in soldierTypes)
        {
            if (!soldierType.IsUnlocked) continue;

            GameObject buttonObj = Instantiate(unitButtonPrefab, unitButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            unitButtons.Add(button);

            TMP_Text[] texts = buttonObj.GetComponentsInChildren<TMP_Text>();
            Image unitImage = buttonObj.transform.Find("Frame_Unit/UnitImage").GetComponent<Image>();
            Image unitCostImage = buttonObj.transform.Find("UnitCostImage").GetComponent<Image>();
            TMP_Text unitCostText = unitCostImage.transform.Find("UnitCostText").GetComponent<TMP_Text>();

            texts[0].text = soldierType.SoldierName;
            unitImage.sprite = soldierType.SoldierIcon;
            unitCostText.text = soldierType.FoodCost.ToString();

            button.onClick.AddListener(() => {
                if (_resourceManager.Food >= soldierType.FoodCost)
                {
                    _gameManager.SpawnSoldier(soldierType);
                    _resourceManager.SpendFood(soldierType.FoodCost);
                    UpdateFoodUI(_resourceManager.Food);
                }
            });
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

    private void UnlockSoldierType(int index)
    {
        if (index < 0 || index >= _gameManager.allSoldierTypes.Count) return;

        SoldierDataSO soldierType = _gameManager.allSoldierTypes[index];
        if (!soldierType.IsUnlocked && _resourceManager.SpendGold(soldierType.UnlockCost))
        {
            soldierType.IsUnlocked = true;
            PlayerPrefs.SetInt(soldierType.SoldierName, 1); // Save unlock status
            PlayerPrefs.Save(); // Save preferences

            UpdateSoldierTypesForEra(currentEra);
            UpdateUpgradeButtonsUI();
        }
    }

    private void UpdateUpgradePanel(List<SoldierDataSO> eraSoldierTypes)
    {
        for (int i = 0; i < soldierCardsUI.Count; i++)
        {
            if (i < eraSoldierTypes.Count)
            {
                SoldierDataSO soldierType = eraSoldierTypes[i];
                soldierCardsUI[i].gameObject.SetActive(true);
                soldierCardsUI[i].Setup(soldierType, _resourceManager, this);
            }
            else
            {
                soldierCardsUI[i].gameObject.SetActive(false);
            }
        }
    }

    public void ShowGameOverPanel(int roundGoldEarned)
    {
        UpdateGameOverRoundGoldUI(roundGoldEarned);
        gameOverPanel.SetActive(true);
    }

    public void CloseGameOverPanel()
    {
        StartCoroutine(RestartSceneWithFade());
    }

    private IEnumerator RestartSceneWithFade()
    {
        yield return StartCoroutine(Fade(0f, 1f, 1f));
        _resourceManager.AddRoundGoldToTotal();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return StartCoroutine(Fade(1f, 0f, 1f));
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration, Action postFadeAction = null, bool reloadScene = false)
    {
        fadeOverlay.gameObject.SetActive(true);
        Color color = fadeOverlay.color;
        float elapsedTime = 0f;
    
        while (elapsedTime < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            fadeOverlay.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        color.a = endAlpha;
        fadeOverlay.color = color;
    
        postFadeAction?.Invoke();
    
        if (reloadScene)
        {
            _resourceManager.SaveTotalGold();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (endAlpha == 0f)
        {
            fadeOverlay.gameObject.SetActive(false);
        }
    }
    
    public void FadeAndReload()
    {
        StartCoroutine(Fade(0f, 1f, 5f, null, true));
    }
    
    private IEnumerator SlideUIElement(GameObject uiElement, int slideAmount,int isDown)
    {
        RectTransform buttonRectTransform = uiElement.GetComponent<RectTransform>();
        float elapsedTime = 0f;
        Vector2 originalPosition = buttonRectTransform.anchoredPosition;
        Vector2 targetPosition = new Vector2(originalPosition.x, originalPosition.y - slideAmount * isDown);

        while (elapsedTime < 5f)
        {
            elapsedTime += Time.deltaTime;
            buttonRectTransform.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, elapsedTime / 1f);
            yield return null;
        }

        CanvasGroup canvasGroupAlpha = uiElement.GetComponent<CanvasGroup>();
        elapsedTime = 0;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            canvasGroupAlpha.alpha = Mathf.Lerp(1f, 0f, elapsedTime / 1f);
            yield return null;
        }
        uiElement.gameObject.SetActive(false);
        uiElement.GetComponent<RectTransform>().anchoredPosition = originalPosition;
        canvasGroupAlpha.alpha = 1f;
    }

    public Image GetFoodFillingImage()
    {
        return foodFillingImage;
    }
    
    public void OnSoldierUnlocked()
    {
        UpdateSoldierTypesForEra(currentEra);
        CreateUnitButtons(soldierTypes.Where(s => s.Era == currentEra && s.IsUnlocked).ToList());
    }
}
