using System;
using System.Collections;
using UnityEngine;

public class Gold : MonoBehaviour
{
    private Vector2 direction;
    private Vector3 screenTargetPosition;

    private ResourceManager _resourceManager;
    private UIManager _uiManager;
    private void Start()
    {
        _resourceManager = FindObjectOfType<ResourceManager>();
        _uiManager = FindObjectOfType<UIManager>();
    }

    public void Initialize(Vector2 initialDirection)
    {
        direction = initialDirection;
        StartCoroutine(_uiManager == null ? RetryInitialize() : MoveToRandomPositionThenUI());
    }

    private IEnumerator RetryInitialize()
    {
        while (_uiManager == null)
        {
            _uiManager = FindObjectOfType<UIManager>();
            yield return null;
        }

        screenTargetPosition = _uiManager.GetGoldUIPosition();
        StartCoroutine(MoveToRandomPositionThenUI());
    }

    public void MoveImmediately()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToUI());
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
        StartCoroutine(MoveToUI());
    }

    private IEnumerator MoveToUI()
    {
        Vector3 worldTargetPosition = Camera.main.ScreenToWorldPoint(screenTargetPosition);
        worldTargetPosition.z = 0;

        while (Vector2.Distance(transform.position, worldTargetPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, worldTargetPosition, 2.5f * Time.deltaTime);
            yield return null;
        }

        _resourceManager.AddRoundGold(1);
        _uiManager.UpdateRoundGoldUI(_resourceManager.RoundGold);
        Destroy(gameObject);
    }
}

