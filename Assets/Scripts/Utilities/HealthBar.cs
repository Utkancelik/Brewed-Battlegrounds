using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image backgroundImage;
    public Image foregroundImage;
    private Coroutine lerpCoroutine;

    private Transform target;

    public void Initialize(Transform target)
    {
        this.target = target;
    }

    public void SetMaxHealth(int health)
    {
        foregroundImage.fillAmount = 1f;
    }

    public void SetHealth(int health, int maxHealth)
    {
        float targetFillAmount = (float)health / maxHealth;
        foregroundImage.fillAmount = targetFillAmount;

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(LerpBackgroundFill(targetFillAmount));
    }

    private IEnumerator LerpBackgroundFill(float targetFillAmount)
    {
        float initialFillAmount = backgroundImage.fillAmount;
        float elapsedTime = 0f;
        float duration = 1f; // Duration to complete the lerp

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            backgroundImage.fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, elapsedTime / duration);
            yield return null;
        }
        backgroundImage.fillAmount = targetFillAmount;
    }
}
