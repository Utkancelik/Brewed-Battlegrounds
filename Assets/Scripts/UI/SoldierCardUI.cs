using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SoldierCardUI : MonoBehaviour
{
    [SerializeField] private Image soldierIcon;
    [SerializeField] private TMP_Text unlockCost;
    [SerializeField] private GameObject unlockButton;
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text healthText;

    private ResourceManager _resourceManager;
    private SoldierDataSO _soldierData;
    private UIManager _uiManager;

    public void Setup(SoldierDataSO soldierData, ResourceManager resourceManager, UIManager uiManager)
    {
        _resourceManager = resourceManager;
        _soldierData = soldierData;
        _uiManager = uiManager;
        
        soldierIcon.sprite = soldierData.SoldierIcon;
        unlockCost.text = $"{soldierData.UnlockCost} Gold";
        unlockButton.SetActive(!soldierData.IsUnlocked);
        statsPanel.SetActive(soldierData.IsUnlocked);

        // Set the color of the unlock cost text based on the player's total gold
        if (_resourceManager.TotalGold >= soldierData.UnlockCost)
        {
            unlockCost.color = Color.green;
        }
        else
        {
            unlockCost.color = Color.red;
        }

        if (soldierData.IsUnlocked)
        {
            damageText.text = soldierData.Damage.ToString();
            healthText.text = soldierData.Health.ToString();
        }

        var buttonComponent = unlockButton.GetComponent<Button>();
        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(UnlockSoldier);
    }

    private void UnlockSoldier()
    {
        if (_resourceManager.SpendGold(_soldierData.UnlockCost))
        {
            _soldierData.IsUnlocked = true;
            PlayerPrefs.SetInt(_soldierData.SoldierName, 1);
            PlayerPrefs.Save();
            unlockButton.SetActive(false);
            statsPanel.SetActive(true);
            _uiManager.OnSoldierUnlocked();
        }
    }
}
