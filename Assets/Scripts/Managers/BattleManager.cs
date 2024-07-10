using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [SerializeField] private GameObject waveTextObject;
    [SerializeField] private Button battleButton;
    [SerializeField] private WaveData waveData;
    [SerializeField] private float waveTextDisplayDuration = 2f; // Duration for wave text to stay on screen
    [SerializeField] private float waveTextStayAfterSpawn = 2f; // Duration for wave text to stay after spawning enemies
    [SerializeField] private float battleButtonSlideDuration = 0.5f; // Duration for the battle button slide down animation

    private SoldierSpawner soldierSpawner;
    private RectTransform battleButtonRectTransform;

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
        battleButton.onClick.AddListener(StartBattle);
        battleButtonRectTransform = battleButton.GetComponent<RectTransform>();
    }

    private void StartBattle()
    {
        StartCoroutine(SpawnWaves());
        StartCoroutine(SlideBattleButtonDown());
    }

    private IEnumerator SpawnWaves()
    {
        int waveCount = waveData.Waves.Count;
        for (int i = 0; i < waveCount; i++)
        {
            DisplayWaveText($"Wave {i + 1}");
            yield return new WaitForSeconds(waveTextDisplayDuration); // Display wave text for a while before spawning

            int totalEnemyInThisWave = waveData.Waves[i].Amount;
            for (int j = 0; j < totalEnemyInThisWave; j++)
            {
                GameObject enemyGameObject = Instantiate(waveData.Waves[i].Soldier.gameObject, soldierSpawner.EnemySpawnArea.GetRandomPosition(), Quaternion.identity);
                Soldier enemySoldier = enemyGameObject.GetComponent<Soldier>();
                enemySoldier.IsEnemy = true;
                yield return new WaitForSeconds(waveData.delayBetweenUnits);
            }

            yield return new WaitForSeconds(waveTextStayAfterSpawn); // Wait a bit before hiding the wave text
            waveTextObject.SetActive(false);

            yield return new WaitForSeconds(waveData.delayBetweenWaves); // Delay between waves
        }
        DisplayWaveText("Final Wave");
    }

    private void DisplayWaveText(string text)
    {
        waveTextObject.GetComponentInChildren<TMP_Text>().text = text;
        waveTextObject.SetActive(true);
    }

    private IEnumerator SlideBattleButtonDown()
    {
        float elapsedTime = 0f;
        Vector2 originalPosition = battleButtonRectTransform.anchoredPosition;
        Vector2 targetPosition = new Vector2(originalPosition.x, originalPosition.y - 1000); // Adjust as needed

        while (elapsedTime < battleButtonSlideDuration)
        {
            elapsedTime += Time.deltaTime;
            battleButtonRectTransform.anchoredPosition = Vector2.Lerp(originalPosition, targetPosition, elapsedTime / battleButtonSlideDuration);
            yield return null;
        }

        battleButton.gameObject.SetActive(false);
    }

    public void SetSoldierSpawner(SoldierSpawner spawner)
    {
        soldierSpawner = spawner;
    }
}
