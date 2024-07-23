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
    
    [Header("General Settings")]
    [SerializeField] private GameObject waveTextObject;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private TMP_Text currentGoldText;
    [SerializeField] private TMP_Text foodText;
    [SerializeField] private TMP_Text totalGoldVaultText;
    [SerializeField] private TMP_Text gameOverRoundGoldText;
    [SerializeField] private TMP_Text roundGoldInGameText;
    [SerializeField] private GameObject unitButtonPrefab;
    [SerializeField] private Transform unitButtonContainer;
    public Button StartBattleButton => startBattleButton;
    
    [Header("Panels")]
    [SerializeField] private GameObject mainBattlePanel;
    [SerializeField] private GameObject battleBottomPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject mainButtonsPanel;
    [Header("Buttons")]
    [SerializeField] private Button goToBattleButton;
    [SerializeField] private Button upgradeButton;

    [SerializeField] private List<Button> unlockSoldierButtons;
    [SerializeField] private List<Image> soldierCards;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Button closeGameOverPanelButton;
    [SerializeField] private Image fadeOverlay;

    [SerializeField] private Button increaseFoodProductionButton;
    [SerializeField] private Button increaseBaseHealthButton;
    [SerializeField] private TMP_Text foodProductionRateText;
    [SerializeField] private TMP_Text baseHealthText;
    [SerializeField] private Image foodFillingImage;
    private List<Button> unitButtons = new List<Button>();

    private ResourceManager _resourceManager;
    private GameManager _gameManager;

    private List<SoldierDataSO> soldierTypes;
    private int currentEra = 1;
    
    [SerializeField] private TMP_Text foodProductionCostText;
    [SerializeField] private TMP_Text baseHealthCostText;
    private bool isFoodProcessed = false;
    
    [SerializeField] private GameObject soldierCardPrefab; // Prefab for soldier card UI
    [SerializeField] private Transform soldiersContainer; // Parent container for soldier cards
    private List<SoldierCardUI> soldierCardsUI = new List<SoldierCardUI>();

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
    
    public void Initialize(List<SoldierDataSO> allSoldierTypes)
    {
        soldierTypes = allSoldierTypes;
        CreateSoldierCards();
        UpdateSoldierTypesForEra(currentEra);
    }

    private void Awake()
    {
        DIContainer.Instance.Register(this);
        
        // Automatically find and assign soldier buttons and cards
        unlockSoldierButtons = new List<Button>();
        soldierCards = new List<Image>();

        foreach (Transform child in soldiersContainer)
        {
            Button unlockButton = child.Find("Unlock").GetComponent<Button>();
            Image soldierCard = child.Find("Icon").GetComponent<Image>();
            unlockSoldierButtons.Add(unlockButton);
            soldierCards.Add(soldierCard);
        }
    }



private void Start()
    {
        _resourceManager = DIContainer.Instance.Resolve<ResourceManager>();
        _gameManager = DIContainer.Instance.Resolve<GameManager>();
        
        foodProductionCostText.text = _resourceManager.foodProductionUpgradeCost.ToString();
        baseHealthCostText.text = _resourceManager.baseHealthUpgradeCost.ToString();
        
        increaseFoodProductionButton.onClick.AddListener(() => _resourceManager.IncreaseFoodProductionRate());
        increaseBaseHealthButton.onClick.AddListener(() => _resourceManager.IncreaseBaseHealth());
        goToBattleButton.onClick.AddListener(EnterBattleScene);
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
        startBattleButton.onClick.AddListener(() => OnStartBattle?.Invoke()); // This is where you call the action
        closeGameOverPanelButton.onClick.AddListener(CloseGameOverPanel);
        
        increaseFoodProductionButton.onClick.AddListener(() => 
        {
            _resourceManager.IncreaseFoodProductionRate();
            UpdateUpgradeButtonsUI();
        });

        increaseBaseHealthButton.onClick.AddListener(() => 
        {
            _resourceManager.IncreaseBaseHealth();
            UpdateUpgradeButtonsUI();
        });
        
        UpdateUpgradeButtonsUI();
        UpdateFoodProductionRateUI(_resourceManager.foodProductionRate);
        UpdateBaseHealthUI(_gameManager.PlayerBase.Health);
        UpdateGoldUI(_resourceManager.Gold);
        UpdateFoodUI(_resourceManager.Food);
        UpdateTotalGoldUI(_resourceManager.TotalGold);
        UpdateRoundGoldUI(_resourceManager.RoundGold);

        _gameManager.allSoldierTypes[0].IsUnlocked = true;

        foreach (var soldierType in _gameManager.allSoldierTypes)
        {
            soldierType.IsUnlocked = PlayerPrefs.GetInt(soldierType.SoldierName, 0) == 1;
        }
        for (int i = 0; i < unlockSoldierButtons.Count; i++)
        {
            int index = i;
            unlockSoldierButtons[i].onClick.AddListener(() => UnlockSoldierType(index));
        }

        EnterBattleScene();
        Initialize(_gameManager.allSoldierTypes);
        _resourceManager.LoadUpgrades();

        gameOverPanel.SetActive(false);
        fadeOverlay.gameObject.SetActive(false);
    }    
    private void CreateSoldierCards()
    {
        foreach (Transform child in soldiersContainer)
        {
            Destroy(child.gameObject); // Clear any existing cards
        }

        soldierCardsUI.Clear();

        for (int i = 0; i < 3; i++) // Assuming you want to display 3 cards at a time
        {
            GameObject cardObj = Instantiate(soldierCardPrefab, soldiersContainer);
            SoldierCardUI soldierCardUI = cardObj.GetComponent<SoldierCardUI>();
            if (soldierCardUI != null)
            {
                soldierCardsUI.Add(soldierCardUI);
            }
            else
            {
                Debug.LogError("SoldierCardUI component is missing on the instantiated prefab.");
            }
        }
    }


    public void UpdateUpgradeButtonsUI()
    {
        UpdateUpgradeButtonColor(foodProductionCostText, _resourceManager.foodProductionUpgradeCost);
        UpdateUpgradeButtonColor(baseHealthCostText, _resourceManager.baseHealthUpgradeCost);

        for (int i = 0; i < soldierCardsUI.Count; i++)
        {
            if (i < _gameManager.allSoldierTypes.Count)
            {
                SoldierCardUI soldierCardUI = soldierCardsUI[i];
                SoldierDataSO soldierType = _gameManager.allSoldierTypes[i];
                TMP_Text costText = soldierCardUI.transform.Find("Cost").GetComponent<TMP_Text>();
                costText.text = soldierType.UnlockCost.ToString();
                UpdateUpgradeButtonColor(costText, soldierType.UnlockCost);
                soldierCardUI.Setup(soldierType, soldierType.IsUnlocked, () => UnlockSoldierType(i));
            }
        }
    }


    
    private void UpdateUpgradeButtonColor(TMP_Text costText, int cost)
    {
        costText.color = _resourceManager.TotalGold >= cost ? Color.green : Color.red;
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
        UpdateSoldierTypesForEra(currentEra);
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
        StartCoroutine(SlideUIElement(waveTextObject.gameObject, -1));
    }

    public void UpdateGoldUI(int goldAmount)
    {
        currentGoldText.text = $"{goldAmount}";
    }

    public void UpdateTotalGoldUI(int totalGoldAmount)
    {
        totalGoldVaultText.text = $"Total Gold: {totalGoldAmount}";
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
            Image unitImage = buttonObj.transform.Find("UnitImage").GetComponent<Image>();
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
                soldierCardsUI[i].Setup(soldierType, soldierType.IsUnlocked, () => UnlockSoldierType(i));
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
        yield return StartCoroutine(FadeToBlack());
        _resourceManager.AddRoundGoldToTotal();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        yield return StartCoroutine(FadeFromBlack());
    }
    
    public void FadeAndReload()
    {
        StartCoroutine(FadeToBlackAndReload());
    }

    private IEnumerator FadeToBlackAndReload()
    {
        fadeOverlay.gameObject.SetActive(true);
        Color color = fadeOverlay.color;
        float fadeDuration = 5f; // Extend duration for a longer fade
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            color.a = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            fadeOverlay.color = color;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        fadeOverlay.color = color;

        _resourceManager.SaveTotalGold();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator FadeToBlack()
    {
        fadeOverlay.gameObject.SetActive(true);
        Color color = fadeOverlay.color;
        while (color.a < 1f)
        {
            color.a += Time.deltaTime;
            fadeOverlay.color = color;
            yield return null;
        }
    }

    private IEnumerator FadeFromBlack()
    {
        Color color = fadeOverlay.color;
        while (color.a > 0f)
        {
            color.a -= Time.deltaTime;
            fadeOverlay.color = color;
            yield return null;
        }
        fadeOverlay.gameObject.SetActive(false);
    }

    private IEnumerator SlideUIElement(GameObject uiElement, int isDown)
    {
        RectTransform buttonRectTransform = uiElement.GetComponent<RectTransform>();
        float elapsedTime = 0f;
        Vector2 originalPosition = buttonRectTransform.anchoredPosition;
        Vector2 targetPosition = new Vector2(originalPosition.x, originalPosition.y - 1000 * isDown);

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

    public IEnumerator FillFoodImage(Image foodImage, float duration)
    {
        if (isFoodProcessed)
        {
            yield break;
        }

        isFoodProcessed = true;
        foodImage.fillAmount = 0f; // Reset fill amount at the start
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            foodImage.fillAmount = Mathf.Lerp(0, 1,  duration);
            elapsedTime += .1f;
            yield return new WaitForSeconds(0.1f);
        }

        foodImage.fillAmount = 1f;
        isFoodProcessed = false;

    }

    public Image GetFoodFillingImage()
    {
        return foodFillingImage;
    }
}
