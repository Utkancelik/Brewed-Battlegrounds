using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject waveTextObject;
    [SerializeField] private Button startBattleButton;
    [SerializeField] private TMP_Text currentGoldText; // Reference to the UI text element displaying current gold
    [SerializeField] private TMP_Text foodText; // Reference to the UI text element displaying food
    [SerializeField] private TMP_Text totalGoldVaultText; // Reference to the UI text element displaying total gold vault
    [SerializeField] private TMP_Text gameOverRoundGoldText; // Reference to the UI text element displaying round gold in the game over panel
    [SerializeField] private TMP_Text roundGoldInGameText; // Reference to the UI text element displaying round gold in-game
    [SerializeField] private GameObject unitButtonPrefab; // Prefab for unit buttons
    [SerializeField] private Transform unitButtonContainer; // Container for unit buttons
    public Button StartBattleButton => startBattleButton;

    [SerializeField] private GameObject mainBattlePanel; // Panel for the battle scene
    [SerializeField] private GameObject battleBottomPanel; // Panel for the battle controls
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private GameObject mainButtonsPanel; // Panel containing the 5 main buttons

    [SerializeField] private Button goToBattleButton; // Button to go to battle scene from main buttons
    [SerializeField] private Button upgradeButton; // Button to go to upgrade panel

    [SerializeField] private List<Button> unlockSoldierButtons; // Buttons to unlock soldier types
    [SerializeField] private List<Image> soldierCards; // Images for soldier cards

    [SerializeField] private GameObject gameOverPanel; // Game Over Panel
    [SerializeField] private Button closeGameOverPanelButton; // Button to close the game over panel
    [SerializeField] private Image fadeOverlay; // Image for fade effect

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
        // Initialize the UI with the current gold and food amount
        UpdateGoldUI(ResourceManager.Instance.Gold);
        UpdateFoodUI(ResourceManager.Instance.Food);
        UpdateTotalGoldUI(ResourceManager.Instance.TotalGold);
        UpdateRoundGoldUI(ResourceManager.Instance.RoundGold);

        // Assign button listeners
        goToBattleButton.onClick.AddListener(EnterBattleScene);
        upgradeButton.onClick.AddListener(OpenUpgradePanel);
        startBattleButton.onClick.AddListener(StartBattle);
        closeGameOverPanelButton.onClick.AddListener(CloseGameOverPanel);

        // Set the first soldier type as unlocked by default
        GameManager.Instance.soldierTypes[0].isUnlocked = true;

        for (int i = 0; i < unlockSoldierButtons.Count; i++)
        {
            int index = i;
            unlockSoldierButtons[i].onClick.AddListener(() => UnlockSoldierType(index));
        }

        // Ensure the MainBattlePanel is active when the game starts
        EnterBattleScene();
        // Ensure the upgrade panel is updated with the initial state
        UpdateUpgradePanel();

        gameOverPanel.SetActive(false); // Ensure the Game Over panel is inactive at the start
        fadeOverlay.gameObject.SetActive(false); // Ensure the fade overlay is inactive at the start
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
        mainButtonsPanel.SetActive(true); // Keep the main buttons visible
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(false);
    }

    public void OpenUpgradePanel()
    {
        mainBattlePanel.SetActive(false);
        battleBottomPanel.SetActive(false);
        upgradePanel.SetActive(true);
        UpdateUpgradePanel(); // Refresh the upgrade panel whenever it's opened
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
        if (currentGoldText != null)
        {
            currentGoldText.text = $"{goldAmount}";
        }
        else
        {
            Debug.LogError("Gold text UI element not assigned in UIManager.");
        }
    }

    public void UpdateTotalGoldUI(int totalGoldAmount)
    {
        if (totalGoldVaultText != null)
        {
            totalGoldVaultText.text = $"Total Gold: {totalGoldAmount}";
        }
        else
        {
            Debug.LogError("Total gold text UI element not assigned in UIManager.");
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

    public void UpdateRoundGoldUI(int roundGoldAmount)
    {
        if (roundGoldInGameText != null)
        {
            roundGoldInGameText.text = $"{roundGoldAmount}";
        }
        else
        {
            Debug.LogError("Round gold text UI element not assigned in UIManager.");
        }
    }

    public void UpdateGameOverRoundGoldUI(int roundGoldAmount)
    {
        if (gameOverRoundGoldText != null)
        {
            gameOverRoundGoldText.text = $"Gold Earned: {roundGoldAmount}";
        }
        else
        {
            Debug.LogError("Game over round gold text UI element not assigned in UIManager.");
        }
    }

    public Vector3 GetGoldUIPosition()
    {
        Vector3 screenPosition = currentGoldText.transform.position;
        return screenPosition;
    }

    public void CreateUnitButtons(List<SoldierType> soldierTypes)
    {
        ClearUnitButtons();

        foreach (var soldierType in soldierTypes)
        {
            if (!soldierType.isUnlocked) continue; // Only create buttons for unlocked soldier types

            GameObject buttonObj = Instantiate(unitButtonPrefab, unitButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            unitButtons.Add(button); // Correct method name with capital 'A'

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

            button.onClick.AddListener(() => {
                if (ResourceManager.Instance.Food >= soldierType.stats.FoodCost) // Check if enough food
                {
                    GameManager.Instance.SpawnSoldier(soldierType);
                    ResourceManager.Instance.SpendFood(soldierType.stats.FoodCost); // Spend the food
                    UpdateFoodUI(ResourceManager.Instance.Food); // Update the UI
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
        if (!soldierType.isUnlocked && ResourceManager.Instance.SpendGold(soldierType.unlockCost))
        {
            soldierType.isUnlocked = true;
            UpdateUpgradePanel();
            CreateUnitButtons(GameManager.Instance.soldierTypes); // Refresh unit buttons
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
    }

    public void ShowGameOverPanel(int roundGoldEarned)
    {
        UpdateGameOverRoundGoldUI(roundGoldEarned); // Use the new method for game over round gold
        gameOverPanel.SetActive(true);
    }

    public void CloseGameOverPanel()
    {
        StartCoroutine(RestartSceneWithFade());
    }

    private IEnumerator RestartSceneWithFade()
    {
        yield return StartCoroutine(FadeToBlack());

        // Add round gold to total gold before reloading the scene
        ResourceManager.Instance.AddRoundGoldToTotal();

        // Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        yield return StartCoroutine(FadeFromBlack());
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
}



