using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public static event Action OnStartBattle;

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

    [SerializeField] private GameObject mainBattlePanel;
    [SerializeField] private GameObject battleBottomPanel;
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject mainButtonsPanel;

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

    private void OnEnable()
    {
        OnStartBattle += StartBattle;
    }

    private void OnDisable()
    {
        OnStartBattle -= StartBattle;
    }

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
        increaseFoodProductionButton.onClick.AddListener(() => ResourceManager.Instance.IncreaseFoodProductionRate());
        increaseBaseHealthButton.onClick.AddListener(() => ResourceManager.Instance.IncreaseBaseHealth());
        goToBattleButton.onClick.AddListener(EnterBattleScene);
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
        startBattleButton.onClick.AddListener(() => OnStartBattle?.Invoke()); // This is where you call the action
        closeGameOverPanelButton.onClick.AddListener(CloseGameOverPanel);

        UpdateFoodProductionRateUI(ResourceManager.Instance.foodProductionRate);
        UpdateBaseHealthUI(GameManager.Instance.PlayerBase.Health);
        UpdateGoldUI(ResourceManager.Instance.Gold);
        UpdateFoodUI(ResourceManager.Instance.Food);
        UpdateTotalGoldUI(ResourceManager.Instance.TotalGold);
        UpdateRoundGoldUI(ResourceManager.Instance.RoundGold);

        GameManager.Instance.soldierTypes[0].IsUnlocked = true;

        foreach (var soldierType in GameManager.Instance.soldierTypes)
        {
            soldierType.IsUnlocked = PlayerPrefs.GetInt(soldierType.SoldierName, 0) == 1;
        }
        for (int i = 0; i < unlockSoldierButtons.Count; i++)
        {
            int index = i;
            unlockSoldierButtons[i].onClick.AddListener(() => UnlockSoldierType(index));
        }

        EnterBattleScene();
        UpdateUpgradePanel();
        CreateUnitButtons(GameManager.Instance.soldierTypes);
        ResourceManager.Instance.LoadUpgrades();

        gameOverPanel.SetActive(false);
        fadeOverlay.gameObject.SetActive(false);
    }

    public void UpdateFoodProductionRateUI(float rate)
    {
        foodProductionRateText.text = $"Food Production Rate: {rate}";
    }

    public void UpdateBaseHealthUI(int health)
    {
        baseHealthText.text = $"Base Health: {health}";
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
        StartCoroutine(SlideUIElement(waveTextObject.gameObject, -1));
    }

    public void HideWaveText()
    {
        waveTextObject.SetActive(false);
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

    public void CreateUnitButtons(List<SoldierType> soldierTypes)
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
            unitCostText.text = soldierType.Stats.FoodCost.ToString();

            button.onClick.AddListener(() => {
                if (ResourceManager.Instance.Food >= soldierType.Stats.FoodCost)
                {
                    GameManager.Instance.SpawnSoldier(soldierType);
                    ResourceManager.Instance.SpendFood(soldierType.Stats.FoodCost);
                    UpdateFoodUI(ResourceManager.Instance.Food);
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
        if (index < 0 || index >= GameManager.Instance.soldierTypes.Count) return;

        SoldierType soldierType = GameManager.Instance.soldierTypes[index];
        if (!soldierType.IsUnlocked && ResourceManager.Instance.SpendGold(soldierType.UnlockCost))
        {
            soldierType.IsUnlocked = true;
            PlayerPrefs.SetInt(soldierType.SoldierName, 1); // Save unlock status
            PlayerPrefs.Save(); // Save preferences

            UpdateUpgradePanel();
            CreateUnitButtons(GameManager.Instance.soldierTypes);
        }
    }

    private void UpdateUpgradePanel()
    {
        for (int i = 0; i < soldierCards.Count; i++)
        {
            if (i < GameManager.Instance.soldierTypes.Count)
            {
                SoldierType soldierType = GameManager.Instance.soldierTypes[i];
                soldierCards[i].color = soldierType.IsUnlocked ? Color.white : Color.gray;
                unlockSoldierButtons[i].gameObject.SetActive(!soldierType.IsUnlocked || i == 0);
                unlockSoldierButtons[i].GetComponentInChildren<TMP_Text>().text = soldierType.IsUnlocked ? "Unlocked" : $"{soldierType.UnlockCost} Gold";
            }
            else
            {
                soldierCards[i].gameObject.SetActive(false);
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
        ResourceManager.Instance.AddRoundGoldToTotal();
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

        ResourceManager.Instance.SaveTotalGold();
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
            uiElement.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, elapsedTime / 1f);
            yield return null;
        }
        uiElement.gameObject.SetActive(false);
        uiElement.GetComponent<RectTransform>().anchoredPosition = originalPosition;
        uiElement.GetComponent<CanvasGroup>().alpha = 1f;
    }

    public IEnumerator FillFoodImage(Image foodImage, float duration)
    {
        foodImage.fillAmount = 0f; // Reset fill amount at the start
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            foodImage.fillAmount = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foodImage.fillAmount = 1f;
    }

    public Image GetFoodFillingImage()
    {
        return foodFillingImage;
    }
}
