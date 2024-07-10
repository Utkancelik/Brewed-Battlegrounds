using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image foregroundImage;
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
        foregroundImage.fillAmount = (float)health / maxHealth;
    }
}