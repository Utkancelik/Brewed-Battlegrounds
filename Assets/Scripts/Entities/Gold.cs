using System.Collections;
using UnityEngine;

public class Gold : MonoBehaviour
{
    private Vector2 direction;
    private RectTransform goldUIRectTransform;
    private Vector3 worldTargetPosition;

    public void Initialize(Vector2 initialDirection)
    {
        direction = initialDirection;
        goldUIRectTransform = UIManager.Instance.GoldPosition.GetComponent<RectTransform>();
        ConvertRectTransformToWorldPosition();
        StartCoroutine(MoveToRandomPositionThenUI());
    }

    private void ConvertRectTransformToWorldPosition()
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, goldUIRectTransform.position);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(goldUIRectTransform, screenPoint, Camera.main, out worldTargetPosition);
    }

    private IEnumerator MoveToRandomPositionThenUI()
    {
        Vector2 randomPosition = (Vector2)transform.position + direction;
        float fallDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fallDuration)
        {
            transform.position = Vector2.Lerp(transform.position, randomPosition, elapsedTime / fallDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1f); // Wait for a while at the fallen position

        while (Vector2.Distance(transform.position, worldTargetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, worldTargetPosition, 5f * Time.deltaTime);
            yield return null;
        }

        GameManager.Instance.AddGold(1);
        Destroy(gameObject);
    }
}

