using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private GameObject waveTextObject;
    [SerializeField] private Button battleButton;
    [SerializeField] private TMP_Text goldText; // Reference to the UI text element displaying gold
    public Button BattleButton => battleButton;
    public Transform GoldPosition;

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
            goldText.text = $"Gold: {goldAmount}";
        }
        else
        {
            Debug.LogError("Gold text UI element not assigned in UIManager.");
        }
    }

    public Vector3 GetGoldUIPosition()
    {
        Vector3 screenPosition = goldText.transform.position;
        return screenPosition;
    }
}

