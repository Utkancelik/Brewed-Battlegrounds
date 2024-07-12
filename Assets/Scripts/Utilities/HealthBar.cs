using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foregroundImage;
    public Image backgroundImage;
    private Transform target;
    private TMP_Text healthText;

    private void Awake()
    {
        healthText = GetComponentInChildren<TMP_Text>();
    }

    public void Initialize(Transform target, int maxHealth)
    {
        this.target = target;
        SetMaxHealth(maxHealth);
        SetHealth(maxHealth, maxHealth); // Set initial health display
    }

    public void SetMaxHealth(int health)
    {
        foregroundImage.fillAmount = 1f;
        backgroundImage.fillAmount = 1f;
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }
    }

    public void SetHealth(int health, int maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = health.ToString();
        }

        float healthPercentage = (float)health / maxHealth;
        foregroundImage.fillAmount = healthPercentage;

        if (backgroundImage.fillAmount > foregroundImage.fillAmount)
        {
            StartCoroutine(SmoothlyDecreaseBackground(healthPercentage));
        }
        else
        {
            backgroundImage.fillAmount = healthPercentage;
        }
    }

    private IEnumerator SmoothlyDecreaseBackground(float targetFill)
    {
        while (backgroundImage.fillAmount > targetFill)
        {
            backgroundImage.fillAmount = Mathf.Lerp(backgroundImage.fillAmount, targetFill, Time.deltaTime * 10f);
            yield return null;
        }

        backgroundImage.fillAmount = targetFill;
    }
}
