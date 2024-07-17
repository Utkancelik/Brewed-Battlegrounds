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
        originalColor = spriteRenderer.color;
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

        spriteRenderer.color = flickerColor == spriteRenderer.color ? Color.white : flickerColor;

        yield return new WaitForSeconds(duration);

        spriteRenderer.color = originalColor;
        isFlickering = false;
    }
}
