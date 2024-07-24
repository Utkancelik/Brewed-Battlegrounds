using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private WaveDataSO waveDataSo;
    [SerializeField] private float waveTextDisplayDuration = 2f;
    [SerializeField] private float waveTextStayAfterSpawn = 2f;
    [SerializeField] private float battleButtonSlideDuration = 0.5f;

    private SoldierSpawner _soldierSpawner;
    private Coroutine _spawnWaveCoroutine;
    private UIManager _uiManager;
    private GameManager _gameManager;
    private ResourceManager _resourceManager;

    private void Awake() => DIContainer.Instance.Register(this);

    private void Start()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        _soldierSpawner = DIContainer.Instance.Resolve<SoldierSpawner>();
        _uiManager = DIContainer.Instance.Resolve<UIManager>();
        _gameManager = DIContainer.Instance.Resolve<GameManager>();
        _resourceManager = DIContainer.Instance.Resolve<ResourceManager>();
    }

    private void OnEnable() => UIManager.OnStartBattle += StartBattle;
    private void OnDisable() => UIManager.OnStartBattle -= StartBattle;

    private void StartBattle()
    {
        _gameManager.StartBattle();
        _resourceManager.StartProduction();
        _spawnWaveCoroutine = StartCoroutine(SpawnWaves());
        StartCoroutine(SlideBattleButtonDown());
    }

    private IEnumerator SpawnWaves()
    {
        for (int i = 0; i < waveDataSo.Waves.Count; i++)
        {
            _uiManager.DisplayWaveText(i == waveDataSo.Waves.Count - 1 ? "Final Wave" : $"Wave {i + 1}");
            yield return new WaitForSeconds(waveTextDisplayDuration);

            foreach (var group in waveDataSo.Waves[i].Groups)
            {
                for (int j = 0; j < group.Amount; j++)
                {
                    _soldierSpawner.SpawnSoldier(group.Soldier.gameObject, true);
                    yield return new WaitForSeconds(waveDataSo.DelayBetweenUnits);
                }
                yield return new WaitForSeconds(group.DelayAfterGroup);
            }

            yield return new WaitForSeconds(waveTextStayAfterSpawn);
            if (i < waveDataSo.Waves.Count - 1)
            {
                yield return new WaitForSeconds(waveDataSo.DelayBetweenWaves);
            }
        }
    }

    private IEnumerator SlideBattleButtonDown()
    {
        RectTransform battleButtonRectTransform = _uiManager.StartBattleButton.GetComponent<RectTransform>();
        float elapsedTime = 0f;
        Vector2 originalPosition = battleButtonRectTransform.anchoredPosition;
        Vector2 targetPosition = new Vector2(originalPosition.x, originalPosition.y - 1000);

        while (elapsedTime < battleButtonSlideDuration)
        {
            elapsedTime += Time.deltaTime;
            battleButtonRectTransform.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, elapsedTime / battleButtonSlideDuration);
            yield return null;
        }

        _uiManager.StartBattleButton.gameObject.SetActive(false);
    }

    public void SetSoldierSpawner(SoldierSpawner spawner) => _soldierSpawner = spawner;

    public void StopSpawning()
    {
        if (_spawnWaveCoroutine != null)
        {
            StopCoroutine(_spawnWaveCoroutine);
            _spawnWaveCoroutine = null;
        }
    }
}
