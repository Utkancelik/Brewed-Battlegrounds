using System.Collections;
using UnityEngine;

public class Gold : MonoBehaviour
{
    private Vector2 direction;
    private Vector3 screenTargetPosition;

    public void Initialize(Vector2 initialDirection)
    {
        direction = initialDirection;
        screenTargetPosition = UIManager.Instance.GetGoldUIPosition();
        StartCoroutine(MoveToRandomPositionThenUI());
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

        yield return new WaitForSeconds(1f);

        Vector3 worldTargetPosition = Camera.main.ScreenToWorldPoint(screenTargetPosition);
        worldTargetPosition.z = 0;

        while (Vector2.Distance(transform.position, worldTargetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, worldTargetPosition, 10f * Time.deltaTime);
            yield return null;
        }

        ResourceManager.Instance.AddRoundGold(1);
        UIManager.Instance.UpdateRoundGoldUI(ResourceManager.Instance.RoundGold);

        Destroy(gameObject);
    }
}

