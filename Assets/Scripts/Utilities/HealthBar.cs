using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foregroundImage;
    public Image backgroundImage;
    private Transform target;

    public void Initialize(Transform target)
    {
        this.target = target;
    }

    public void SetMaxHealth(int health)
    {
        foregroundImage.fillAmount = 1f;
        backgroundImage.fillAmount = 1f;
    }

    public void SetHealth(int health, int maxHealth)
    {
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