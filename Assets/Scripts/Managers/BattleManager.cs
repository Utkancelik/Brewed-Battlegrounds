using UnityEngine;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] private WaveData waveData;
    [SerializeField] private float waveTextDisplayDuration = 2f;
    [SerializeField] private float waveTextStayAfterSpawn = 2f;
    [SerializeField] private float battleButtonSlideDuration = 0.5f;

    private SoldierSpawner soldierSpawner;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UIManager.Instance.BattleButton.onClick.AddListener(StartBattle);
    }

    private void StartBattle()
    {
        GameManager.Instance.StartBattle(); // Notify GameManager to start production and spawning
        StartCoroutine(SpawnWaves());
        StartCoroutine(SlideBattleButtonDown());
    }

    private IEnumerator SpawnWaves()
    {
        int waveCount = waveData.Waves.Count;
        for (int i = 0; i < waveCount; i++)
        {
            UIManager.Instance.DisplayWaveText(i == waveCount - 1 ? "Final Wave" : $"Wave {i + 1}");
            yield return new WaitForSeconds(waveTextDisplayDuration);

            foreach (var group in waveData.Waves[i].Groups)
            {
                for (int j = 0; j < group.Amount; j++)
                {
                    GameObject enemyGameObject = Instantiate(group.Soldier.gameObject, soldierSpawner.EnemySpawnArea.GetRandomPosition(), Quaternion.identity);
                    Soldier enemySoldier = enemyGameObject.GetComponent<Soldier>();
                    enemySoldier.IsEnemy = true;
                    yield return new WaitForSeconds(waveData.delayBetweenUnits);
                }

                yield return new WaitForSeconds(group.delayAfterGroup);
            }

            yield return new WaitForSeconds(waveTextStayAfterSpawn);
            UIManager.Instance.HideWaveText();

            if (i < waveCount - 1)
            {
                yield return new WaitForSeconds(waveData.delayBetweenWaves);
            }
        }
    }

    private IEnumerator SlideBattleButtonDown()
    {
        RectTransform battleButtonRectTransform = UIManager.Instance.BattleButton.GetComponent<RectTransform>();
        float elapsedTime = 0f;
        Vector2 originalPosition = battleButtonRectTransform.anchoredPosition;
        Vector2 targetPosition = new Vector2(originalPosition.x, originalPosition.y - 1000);

        while (elapsedTime < battleButtonSlideDuration)
        {
            elapsedTime += Time.deltaTime;
            battleButtonRectTransform.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, elapsedTime / battleButtonSlideDuration);
            yield return null;
        }

        UIManager.Instance.BattleButton.gameObject.SetActive(false);
    }

    public void SetSoldierSpawner(SoldierSpawner spawner)
    {
        soldierSpawner = spawner;
    }
}
