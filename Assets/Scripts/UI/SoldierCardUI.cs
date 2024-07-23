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

    public void Setup(SoldierDataSO soldierData, bool isUnlocked, UnityAction unlockAction)
    {
        soldierIcon.sprite = soldierData.SoldierIcon;
        unlockCost.text = $"{soldierData.UnlockCost} Gold";
        unlockButton.SetActive(!isUnlocked);
        statsPanel.SetActive(isUnlocked);

        if (isUnlocked)
        {
            damageText.text = soldierData.Damage.ToString();
            healthText.text = soldierData.Health.ToString();
        }

        var buttonComponent = unlockButton.GetComponent<Button>();
        buttonComponent.onClick.RemoveAllListeners();
        buttonComponent.onClick.AddListener(unlockAction);
    }
}