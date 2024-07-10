using System.Collections;
using UnityEngine;

public class DamageFlicker : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isFlickering = false;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void Flicker(float duration, Color flickerColor)
    {
        if (!isFlickering)
        {
            StartCoroutine(FlickerCoroutine(duration, flickerColor));
        }
    }

    private IEnumerator FlickerCoroutine(float duration, Color flickerColor)
    {
        isFlickering = true;

        // If the current color is already the flicker color, use a different color for the flicker
        if (spriteRenderer.color == flickerColor)
        {
            flickerColor = Color.white;
        }

        spriteRenderer.color = flickerColor;

        yield return new WaitForSeconds(duration);

        spriteRenderer.color = originalColor;
        isFlickering = false;
    }
}